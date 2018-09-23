using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace dbRad.Classes
{
    class DatabaseDataOps
    {
        public static void dbGetDataGridRows(Window winNew, Int32 applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
            NpgsqlConnection controlDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("Control"));
            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(WindowTasks.winMetadataList(applicationTableId).ApplicationName));

            DataTable winDt = new DataTable();

            string sqlPart;
            Int32 sqlParam = applicationTableId;

            //Single row to return user defined DML SQL for DataGrid
            sqlPart =
              @"SELECT Dml
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
                sqlParam = selectedFilter;
                sqlPart =
                  @"SELECT FilterDefinition
                    FROM metadata.ApplicationFilter
                    WHERE ApplicationFilterId = @sqlparam";
            }

            string fltTxt = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam);

            winNew.Resources.Remove("winFilter");
            winNew.Resources.Add("winFilter", fltTxt);
            //Build where clause with replacement values for |COLUMN_NAME| parameters  
            fltTxt = WindowDataOps.SubstituteWindowParameters(fltTxt, controlValues);
            sqlTxt = sqlTxt + " WHERE " + fltTxt;
            string sqlCountText = sqlTxt;
            sqlTxt = sqlTxt + " ORDER BY 1 OFFSET " + tbOffset.Text + " ROWS FETCH NEXT " + tbFetch.Text + " ROWS ONLY";


            try
            {
                {
                    controlDb.Open();
                    applicationDb.Open();
                    {
                        //Run the SQL cmd to return SQL that fills DataGrid
                        NpgsqlCommand execTabSql = applicationDb.CreateCommand();
                        execTabSql.CommandText = sqlTxt;

                        //Create an adapter and fill the grid using sql and adapater
                        NpgsqlDataAdapter winDa = new NpgsqlDataAdapter(execTabSql);
                        winDa.Fill(winDt);
                        winDg.ItemsSource = winDt.DefaultView;

                        //set the page counter
                        string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
                        string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
                        Int32 rowCount = 0;

                        Int32 chrStart = sqlCountText.IndexOf("SELECT") + 6;
                        Int32 chrEnd = sqlCountText.IndexOf("FROM");

                        sqlTxt = sqlCountText.Substring(0, chrStart) + "  COUNT(*) " + sqlCountText.Substring(chrEnd);
                        NpgsqlCommand countRows = new NpgsqlCommand(sqlTxt, applicationDb);
                        rowCount = Convert.ToInt32(countRows.ExecuteScalar());
                        Int32 pageSize = Convert.ToInt32(tbFetch.Text);
                        Int32 offSet = Convert.ToInt32(tbOffset.Text);

                        string pageCount = Convert.ToString((rowCount / pageSize) + 1);
                        string pageNumber = Convert.ToString((offSet / pageSize) + 1);


                        tbSelectorText.Text = "Page " + pageNumber + " of " + pageCount;
                        //Define the grid columns

                        controlDb.Close();
                        applicationDb.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR in Filter SQL:" + ex.Message, sqlTxt);
                controlDb.Close();
                applicationDb.Close();
            }
        }

        public static void dbCreateRecord(Window winNew, Int32 applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
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
            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(applicationName));
            try
            {
                foreach (FrameworkElement element in editStkPnl.Children)
                {
                    string ctlType = element.GetType().Name;

                    if (element.Name != tableKey & ctlType != "Lable")
                    {
                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                if (tb.Tag.ToString() != "NUM")
                                {
                                    columnUpdates.Add("'" + tb.Text + "'");
                                }
                                else
                                {

                                    if (tb.Text == "")
                                    {
                                        tb.Text = "0";
                                    }
                                    columnUpdates.Add(tb.Text);
                                }
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
                sql = "INSERT INTO " + schemaName + "." + tableName + " " + csvColumns + csvColumnUpdates;

                NpgsqlCommand dbCreateRecordSql = new NpgsqlCommand();
                dbCreateRecordSql.CommandText = sql;
                dbCreateRecordSql.CommandType = CommandType.Text;
                dbCreateRecordSql.Connection = applicationDb;

                applicationDb.Open();
                dbCreateRecordSql.ExecuteNonQuery();
                applicationDb.Close();

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Insert Record:" + ex.Message, sql);
                applicationDb.Close();
            }
        }

        public static void dbUpdateRecord(Window winNew, Int32 applicationTableId, DataGrid winDg, StackPanel editStkPnl, StackPanel fltStkPnl, Int32 seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //updates the database with values in the data edit fields
        {
            string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(applicationName));

            string sql = string.Empty;

            try
            {
                Int32 selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                DataTable winSelectedRowDataTable = WindowDataOps.dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);

                Boolean isDirty = false;

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    sql = "UPDATE " + schemaName + "." + tableName + " SET ";
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
                                    if (tb.Tag.ToString() != "NUM")
                                    {
                                        sql = sql + col.ColumnName + " = '" + tb.Text + "', ";

                                    }
                                    else
                                    {
                                        if (row[col].ToString() == "")
                                        {
                                            tb.Text = "0";
                                        }
                                        sql = sql + col.ColumnName + " = " + tb.Text + ", ";
                                    }
                                    isDirty = true;
                                }
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
                        listItemSaveSql.Connection = applicationDb;

                        applicationDb.Open();
                        listItemSaveSql.ExecuteNonQuery();
                        applicationDb.Close();

                        dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                        TextBox tabIdCol = (TextBox)editStkPnl.FindName(tableKey.ToLower());
                        selectedRowIdVal = Convert.ToInt32(tabIdCol.Text);

                        WindowTasks.winDataGridSelectRow(selectedRowIdVal, winDg);

                    };


                }

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Save Record:" + ex.Message, sql);
                applicationDb.Close();
            };
        }

        public static void dbDeleteRecord(Window winNew, Int32 applicationTableId, StackPanel fltStkPnl, DataGrid winDg, StackPanel editStkPnl, Int32 seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //deletes the selected row from the database
        {


            string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(applicationName));

            string sql = string.Empty;
            try
            {
                Int32 selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
                DataTable winSelectedRowDataTable = WindowDataOps.dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);
                //Delete the selected row from db

                sql = "DELETE FROM " + schemaName + "." + tableName + " WHERE " + tableKey + " = @Id";

                NpgsqlCommand delRowSql = new NpgsqlCommand();
                delRowSql.CommandText = sql;
                delRowSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                delRowSql.CommandType = CommandType.Text;
                delRowSql.Connection = applicationDb;

                applicationDb.Open();
                delRowSql.ExecuteNonQuery();
                applicationDb.Close();

                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, false);

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Delete Record:" + ex.Message, sql);
                applicationDb.Close();
            };
        }

    }
}
