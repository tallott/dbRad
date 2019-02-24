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
        public static void dbGetDataGridRows(Window winNew, WindowMetaList windowMetaList, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
                       DataTable winDt = new DataTable();

            string sqlPart;
            Int32 sqlParam = windowMetaList.TableId;

            //Single row to return user defined DML SQL for DataGrid
            sqlPart = ControlDatabaseSql.TableDml();

            string sqlTxt = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam, windowMetaList);

            //Append filter where clause to the end of DML
            if (selectedFilter == 0) //Default filter selected
            {
                sqlPart = ControlDatabaseSql.TableFilterDefault();
            }
            else //Custom filter selected
            {
                sqlParam = selectedFilter;
                sqlPart = ControlDatabaseSql.TableFilterSelected();
            }

            string fltTxt = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam, windowMetaList);

            winNew.Resources.Remove("winFilter");
            winNew.Resources.Add("winFilter", fltTxt);

            //Single row to return user defined sort cols for DataGrid
            sqlParam = windowMetaList.TableId;
            sqlPart = ControlDatabaseSql.TableOrderBy();

            //Get order by
            string sqlOrderBy = WindowDataOps.winDataGridGetBaseSql(sqlPart, sqlParam, windowMetaList);

            //get where clause
            fltTxt = WindowDataOps.SubstituteWindowParameters(fltTxt, controlValues);

            //Build where clause with replacement values for |COLUMN_NAME| parameters  
            sqlTxt = sqlTxt + " WHERE " + fltTxt;

            string sqlCountText = sqlTxt;

            sqlTxt = sqlTxt + " ORDER BY " + sqlOrderBy + " OFFSET " + tbOffset.Text + " ROWS FETCH NEXT " + tbFetch.Text + " ROWS ONLY";

            try
            {
                {
                    windowMetaList.controlDb.Open();
                    windowMetaList.applicationDb.Open();
                    {
                        //Run the SQL cmd to return SQL that fills DataGrid
                        NpgsqlCommand execTabSql = windowMetaList.applicationDb.CreateCommand();
                        execTabSql.CommandText = sqlTxt;

                        //Create an adapter and fill the grid using sql and adapater
                        NpgsqlDataAdapter winDa = new NpgsqlDataAdapter(execTabSql);
                        winDa.Fill(winDt);
                        winDg.ItemsSource = winDt.DefaultView;

                        //set the page counter
                        Int32 rowCount = 0;

                        Int32 chrStart = sqlCountText.IndexOf("SELECT") + 6;
                        Int32 chrEnd = sqlCountText.IndexOf("FROM");

                        sqlTxt = sqlCountText.Substring(0, chrStart) + "  COUNT(*) " + sqlCountText.Substring(chrEnd);
                        NpgsqlCommand countRows = new NpgsqlCommand(sqlTxt, windowMetaList.applicationDb);
                        rowCount = Convert.ToInt32(countRows.ExecuteScalar());
                        Int32 pageSize = Convert.ToInt32(tbFetch.Text);
                        Int32 offSet = Convert.ToInt32(tbOffset.Text);

                        string pageCount = Convert.ToString((rowCount / pageSize) + 1);
                        string pageNumber = Convert.ToString((offSet / pageSize) + 1);


                        tbSelectorText.Text = "Page " + pageNumber + " of " + pageCount;

                        windowMetaList.controlDb.Close();
                        windowMetaList.applicationDb.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR in Filter SQL:" + ex.Message, sqlTxt);
                windowMetaList.controlDb.Close();
                windowMetaList.applicationDb.Close();
            }
        }

        public static Boolean dbCreateRecord(Window winNew, WindowMetaList windowMetaList, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Creates a new record in the db
        {
            List<string> columns = new List<string>();
            List<string> columnUpdates = new List<string>();

            string sql = string.Empty;

            try
            {
                foreach (FrameworkElement element in editStkPnl.Children)
                {
                    string ctlType = element.GetType().Name;

                    if (element.Name != windowMetaList.TableKey & ctlType != "Label" & element.IsEnabled == true)
                    {
                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                if (tb.Tag.ToString() != "NUM")
                                {
                                    columnUpdates.Add("'" + tb.Text.Replace("'", "''") + "'");
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
                                columnUpdates.Add("CAST(" + ((bool)chk.IsChecked ? 1 : 0).ToString() + " AS boolean)");
                                break;

                        };

                    }
                }
                string csvColumns = "(" + String.Join(",", columns) + ")";
                string csvColumnUpdates = " VALUES(" + String.Join(",", columnUpdates) + ")";
                sql = "INSERT INTO " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " " + csvColumns + csvColumnUpdates;

                NpgsqlCommand dbCreateRecordSql = new NpgsqlCommand();
                dbCreateRecordSql.CommandText = sql;
                dbCreateRecordSql.CommandType = CommandType.Text;
                dbCreateRecordSql.Connection = windowMetaList.applicationDb;

                windowMetaList.applicationDb.Open();
                dbCreateRecordSql.ExecuteNonQuery();
                windowMetaList.applicationDb.Close();

                return true;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Insert Record:" + ex.Message, sql);
                windowMetaList.applicationDb.Close();
                return false;
            }
        }

        public static Boolean dbUpdateRecord(WindowMetaList windowMetaList, DataGrid winDg, StackPanel editStkPnl)
        //updates the database with values in the data edit fields

        {

            string sql = string.Empty;

            try
            {
                Int32 selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                DataTable winSelectedRowDataTable = WindowDataOps.dbGetDataRow(windowMetaList, selectedRowIdVal, editStkPnl);

                Boolean isDirty = false;

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    sql = "UPDATE " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " SET ";
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
                                        sql = sql + col.ColumnName + " = '" + tb.Text.Replace("'", "''") + "', ";

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
                                if (cb.SelectedValue != null)
                                {
                                    if (cb.SelectedValue.ToString() != row[col].ToString())
                                    {
                                        sql = sql + col.ColumnName + " = " + cb.SelectedValue + ", ";
                                        isDirty = true;
                                    }
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
                                    sql = sql + col.ColumnName + " = " + Convert.ToBoolean(chk.IsChecked) + ", ";
                                    isDirty = true;
                                }
                                    ;
                                break;
                        };

                    }
                    if (isDirty)
                    { //Update the selected record in database

                        sql = sql.Trim(',', ' ') + " WHERE " + windowMetaList.TableKey + " = @Id";

                        NpgsqlCommand listItemSaveSql = new NpgsqlCommand();
                        listItemSaveSql.CommandText = sql;
                        listItemSaveSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                        listItemSaveSql.CommandType = CommandType.Text;
                        listItemSaveSql.Connection = windowMetaList.applicationDb;

                        windowMetaList.applicationDb.Open();
                        listItemSaveSql.ExecuteNonQuery();
                        windowMetaList.applicationDb.Close();
                    };


                }
                return true;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Save Record:" + ex.Message, sql);
                windowMetaList.applicationDb.Close();
                return false;
            };
        }

        public static void dbDeleteRecord(WindowMetaList windowMetaList, DataGrid winDg)

        //deletes the selected row from the database

        {

            string sql = string.Empty;
            try
            {
                Int32 selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                //Delete the selected row from db
                sql = "DELETE FROM " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " WHERE " + windowMetaList.TableKey + " = @Id";

                NpgsqlCommand delRowSql = new NpgsqlCommand();
                delRowSql.CommandText = sql;
                delRowSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                delRowSql.CommandType = CommandType.Text;
                delRowSql.Connection = windowMetaList.applicationDb;

                windowMetaList.applicationDb.Open();
                delRowSql.ExecuteNonQuery();
                windowMetaList.applicationDb.Close();

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Delete Record:" + ex.Message, sql);
                windowMetaList.applicationDb.Close();
            };
        }

    }
}
