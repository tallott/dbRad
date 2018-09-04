using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace dbRad.Classes
{
    class DatabaseDataOps
    {
        public static void dbGetDataGridRows(Window winNew, String applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> columnValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
            NpgsqlConnection appDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("Control"));

            DataTable winDt = new DataTable();
            
            string sqlPart;
            string sqlParam = applicationTableId;

            //Single row to return user defined DML SQL for DataGrid
            sqlPart =
              @"SELECT TOP 1 Dml
                FROM metadata.ApplicationTable
                WHERE ApplicationTableId = @sqlParam";

            string sqlTxt = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam);

            //Append filter where clause to the end of DML
            if (selectedFilter == 0) //Default filter selected
            {
                sqlPart =
                  @"SELECT FilterDefinition
                    FROM metadata.ApplicationFilter
                    WHERE ApplicationTableId = @sqlparam
                            AND SortOrder = 1";
            }
            else //Custom filter selected
            {
                sqlParam = selectedFilter.ToString();
                sqlPart =
                  @"SELECT FilterDefinition
                    FROM metadata.ApplicationFilter
                    WHERE ApplicationFilterId = @sqlparam";
            }

            string fltTxt = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam);

            winNew.Resources.Remove("winFilter");
            winNew.Resources.Add("winFilter", fltTxt);
            //Build where clause with replacement values for $COLUMN_NAME$ parameters  
            fltTxt = WindowDataOps.SubstituteWindowParameters(editStkPnl, fltTxt, columnValues);
            sqlTxt = sqlTxt + " WHERE " + fltTxt;
            string sqlCountText = sqlTxt;
            sqlTxt = sqlTxt + " ORDER BY 1 OFFSET " + tbOffset.Text + " ROWS FETCH NEXT " + tbFetch.Text + " ROWS ONLY";

            try
            {
                {
                    appDbCon.Open();
                    {
                        //Run the SQL cmd to return SQL that fills DataGrid
                        NpgsqlCommand execTabSql = appDbCon.CreateCommand();
                        execTabSql.CommandText = sqlTxt;

                        //Create an adapter and fill the grid using sql and adapater
                        NpgsqlDataAdapter winDa = new NpgsqlDataAdapter(execTabSql);
                        winDa.Fill(winDt);
                        winDg.ItemsSource = winDt.DefaultView;

                        //set the page counter
                        string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
                        string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
                        int rowCount = 0;

                        int chrStart = sqlCountText.IndexOf("SELECT") + 6;
                        int chrEnd = sqlCountText.IndexOf("FROM");

                        sqlTxt = sqlCountText.Substring(0, chrStart) + "  COUNT(*) " + sqlCountText.Substring(chrEnd);
                        NpgsqlCommand countRows = new NpgsqlCommand(sqlTxt, appDbCon);
                        rowCount = (int)countRows.ExecuteScalar();
                        int pageSize = Convert.ToInt32(tbFetch.Text);
                        int offSet = Convert.ToInt32(tbOffset.Text);

                        string pageCount = Convert.ToString((rowCount / pageSize) + 1);
                        string pageNumber = Convert.ToString((offSet / pageSize) + 1);


                        tbSelectorText.Text = "Page " + pageNumber + " of " + pageCount;
                        //Define the grid columns

                        appDbCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR in Filter SQL:" + ex.Message, sqlTxt);
                appDbCon.Close();
            }
        }

        public static void dbCreateRecord(Window winNew, String applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Creates a new record in the db
        {
            string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            List<string> columns = new List<string>();
            List<string> columnUpdates = new List<string>();

            string sql = string.Empty;
            NpgsqlConnection appDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("Control"));
            try
            {
                foreach (FrameworkElement element in editStkPnl.Children)
                {
                    if (element.Name != tableKey)
                    {

                        string ctlType = element.GetType().Name;
                        // MessageBox.Show(element.Name);
                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                columnUpdates.Add("'" + tb.Text + "'");
                                break;
                            case "ComboBox":

                                ComboBox cb = (ComboBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                columnUpdates.Add(cb.SelectedValue.ToString());
                                break;
                            case "DatePicker":
                                DatePicker dtp = (DatePicker)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                if (dtp.SelectedDate != null)
                                {
                                    columnUpdates.Add("'" + Convert.ToDateTime(dtp.SelectedDate).ToString("yyyy-MM-dd") + "'");
                                }
                                else
                                {
                                    columnUpdates.Add("NULL");
                                }

                                break;
                            case "CheckBox":
                                CheckBox chk = (CheckBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                columnUpdates.Add(((bool)chk.IsChecked ? 1 : 0).ToString());
                                break;
                        };

                    }
                }
                string csvColumns = "(" + String.Join(",", columns) + ")";
                string csvColumnUpdates = " VALUES(" + String.Join(",", columnUpdates) + ")";
                sql = "INSERT INTO [" + schemaName + "].[" + tableName + "]" + csvColumns + csvColumnUpdates;

                NpgsqlCommand dbCreateRecordSql = new NpgsqlCommand();
                dbCreateRecordSql.CommandText = sql;
                dbCreateRecordSql.CommandType = CommandType.Text;
                dbCreateRecordSql.Connection = appDbCon;

                appDbCon.Open();
                dbCreateRecordSql.ExecuteNonQuery();
                appDbCon.Close();

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Insert Record:" + ex.Message, sql);
                appDbCon.Close();
            }
        }

        public static void dbUpdateRecord(Window winNew, String applicationTableId, DataGrid winDg, StackPanel editStkPnl, StackPanel fltStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //updates the database with values in the data edit fields
        {
                        string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            NpgsqlConnection appDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(applicationName));

            string sql = string.Empty;

            try
            {
                string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                DataTable winSelectedRowDataTable = WindowDataOps.dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);

                Boolean isDirty = false;

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    sql = "UPDATE [" + schemaName + "].[" + tableName + "] SET ";
                    foreach (DataColumn col in winSelectedRowDataTable.Columns)
                    {

                        //Build the SQL Statement to update changed values

                        //Determine the Type of control
                        object obj = editStkPnl.FindName(col.ColumnName);

                        string ctlName = obj.GetType().Name;
                        //Use Type to work out how to process value;
                        switch (ctlName)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(col.ColumnName);

                                if (tb.Text.ToString() != row[col].ToString())
                                {
                                    sql = sql + col.ColumnName + " = '" + tb.Text + "', ";
                                    isDirty = true;
                                };
                                break;

                            case "ComboBox":

                                ComboBox cb = (ComboBox)editStkPnl.FindName(col.ColumnName);
                                if (cb.SelectedValue.ToString() != row[col].ToString())
                                {
                                    sql = sql + col.ColumnName + " = " + cb.SelectedValue + ", ";
                                    isDirty = true;
                                }
                                break;

                            case "DatePicker":
                                DatePicker dtp = (DatePicker)editStkPnl.FindName(col.ColumnName);
                                if (row[col].ToString() != "" && dtp.SelectedDate != null)

                                {
                                    if (Convert.ToDateTime(dtp.SelectedDate) != Convert.ToDateTime(row[col]))
                                    {
                                        sql = sql + col.ColumnName + " = '" + Convert.ToDateTime(dtp.SelectedDate).ToString("yyyy-MM-dd") + "', ";
                                        isDirty = true;
                                    }
                                }
                                else if (row[col].ToString() == "" && dtp.SelectedDate != null)
                                {
                                    sql = sql + col.ColumnName + " = '" + Convert.ToDateTime(dtp.SelectedDate).ToString("yyyy-MM-dd") + "', ";
                                    isDirty = true;
                                }
                                    ;
                                break;

                            case "CheckBox":
                                CheckBox chk = (CheckBox)editStkPnl.FindName(col.ColumnName);
                                if (Convert.ToBoolean(chk.IsChecked) != Convert.ToBoolean(row[col]))
                                {
                                    sql = sql + col.ColumnName + " = '" + Convert.ToBoolean(chk.IsChecked) + "', ";
                                    isDirty = true;
                                }
                                    ;
                                break;
                        };

                    }
                    if (isDirty)
                    { //Update the selected record in database

                        sql = sql.Trim(',', ' ') + " WHERE " + tableKey + " = @Id";

                        NpgsqlCommand listItemSaveSql = new NpgsqlCommand();
                        listItemSaveSql.CommandText = sql;
                        listItemSaveSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                        listItemSaveSql.CommandType = CommandType.Text;
                        listItemSaveSql.Connection = appDbCon;

                        appDbCon.Open();
                        listItemSaveSql.ExecuteNonQuery();
                        appDbCon.Close();

                        dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                        TextBox tabIdCol = (TextBox)editStkPnl.FindName(tableKey);
                        selectedRowIdVal = tabIdCol.Text;

                        WindowTasks.winDataGridSelectRow(selectedRowIdVal, winDg);

                    };


                }

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Save Record:" + ex.Message, sql);
                appDbCon.Close();
            };
        }

        public static void dbDeleteRecord(Window winNew, String applicationTableId, StackPanel fltStkPnl, DataGrid winDg, StackPanel editStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //deletes the selected row from the database
        {


            string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            NpgsqlConnection appDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(applicationName));

            string sql = string.Empty;
            try
            {
                string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
                DataTable winSelectedRowDataTable = WindowDataOps.dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);
                //Delete the selected row from db

                sql = "DELETE FROM [" + schemaName + "].[" + tableName + "] WHERE " + tableKey + " = @Id";

                NpgsqlCommand delRowSql = new NpgsqlCommand();
                delRowSql.CommandText = sql;
                delRowSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                delRowSql.CommandType = CommandType.Text;
                delRowSql.Connection = appDbCon;

                appDbCon.Open();
                delRowSql.ExecuteNonQuery();
                appDbCon.Close();

                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, false);

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Delete Record:" + ex.Message, sql);
                appDbCon.Close();
            };
        }

    }
}
