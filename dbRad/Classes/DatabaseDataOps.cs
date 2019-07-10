using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Npgsql;

namespace dbRad.Classes
{
    class DatabaseDataOps
    {
        public static void DbGetDataGridRows(Window winNew, WindowMetaList windowMetaList, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
            DataTable winDt = new DataTable();

            string sqlPart;
            Int32 sqlParam = windowMetaList.TableId;

            string sqlTxt = windowMetaList.TableDml;


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

            //Set Filter 
            windowMetaList.TableFilter = WindowDataOps.SubstituteWindowParameters(WindowDataOps.WinDataGridGetBaseSql(sqlPart, sqlParam, windowMetaList), controlValues);

            sqlParam = windowMetaList.TableId;


            //Set order by
            string sqlOrderBy = windowMetaList.TableOrderBy;

            //Build where clause with replacement values for |COLUMN_NAME| parameters  
            sqlTxt = sqlTxt + " WHERE " + windowMetaList.TableFilter;

            //Save SQl for counting rows
            string sqlCountText = sqlTxt;

            //Add Order by
            sqlTxt = sqlTxt + " ORDER BY " + sqlOrderBy + " OFFSET " + tbOffset.Text + " ROWS FETCH NEXT " + windowMetaList.PageRowCount + " ROWS ONLY";

            try
            {
                windowMetaList.ControlDb.Open();
                windowMetaList.ApplicationDb.Open();
                {
                    //Run the SQL cmd to return SQL that fills DataGrid
                    NpgsqlCommand execTabSql = windowMetaList.ApplicationDb.CreateCommand();
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
                    NpgsqlCommand countRows = new NpgsqlCommand(sqlTxt, windowMetaList.ApplicationDb);
                    rowCount = Convert.ToInt32(countRows.ExecuteScalar());
                    Int32 pageSize = Convert.ToInt32(windowMetaList.PageRowCount);
                    Int32 offSet = Convert.ToInt32(tbOffset.Text);

                    string pageCount = Convert.ToString((rowCount / pageSize) + 1);
                    string pageNumber = Convert.ToString((offSet / pageSize) + 1);

                    tbSelectorText.Text = "Page " + pageNumber + " of " + pageCount;

                    windowMetaList.ControlDb.Close();
                    windowMetaList.ApplicationDb.Close();
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR in DataGrid SQL:" + ex.Message, sqlTxt);
                windowMetaList.ControlDb.Close();
                windowMetaList.ApplicationDb.Close();
            }
        }

        public static Boolean DbCreateRecord(Window winNew, WindowMetaList windowMetaList, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Creates a new record in the db
        {
            List<string> columns = new List<string>();
            List<string> columnUpdates = new List<string>();

            string sql = string.Empty;
            string ctlName = string.Empty;
            string ctlType = string.Empty;
            FrameworkElement e;
            try
            {
                try
                {
                    foreach (FrameworkElement element in editStkPnl.Children)
                    {
                        e = element;
                        ctlType = e.GetType().Name;
                        ctlName = e.Name;

                        if (ctlName != windowMetaList.TableKey & ctlType != "Label" & e.IsEnabled == true)
                        {
                            switch (ctlType)
                            {
                                case "TextBox":
                                    TextBox tb = (TextBox)editStkPnl.FindName(ctlName);
                                    columns.Add(ctlName);
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
                                    ComboBox cb = (ComboBox)editStkPnl.FindName(ctlName);
                                    columns.Add(ctlName);
                                    columnUpdates.Add(cb.SelectedValue.ToString());
                                    break;

                                case "DatePicker":
                                    DatePicker dtp = (DatePicker)editStkPnl.FindName(ctlName);
                                    columns.Add(ctlName);
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
                                    CheckBox chk = (CheckBox)editStkPnl.FindName(ctlName);
                                    columns.Add(ctlName);
                                    columnUpdates.Add("CAST(" + ((bool)chk.IsChecked ? 1 : 0).ToString() + " AS boolean)");
                                    break;
                            }
                        }

                    }

                }
                catch (NullReferenceException ex)
                {
                    WindowTasks.DisplayMessage("Try entering a value for: " + ctlName);

                    return false;
                }


                string csvColumns = "(" + String.Join(",", columns) + ")";
                string csvColumnUpdates = " VALUES(" + String.Join(",", columnUpdates) + ")";
                sql = "INSERT INTO " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " " + csvColumns + csvColumnUpdates;

                NpgsqlCommand dbCreateRecordSql = new NpgsqlCommand
                {
                    CommandText = sql,
                    CommandType = CommandType.Text,
                    Connection = windowMetaList.ApplicationDb
                };

                windowMetaList.ApplicationDb.Open();
                dbCreateRecordSql.ExecuteNonQuery();
                windowMetaList.ApplicationDb.Close();

                return true;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Insert Record:" + ex.Message, sql);
                windowMetaList.ApplicationDb.Close();
                return false;
            }
        }

        public static Boolean DbUpdateRecord(WindowMetaList windowMetaList, DataGrid winDg, StackPanel editStkPnl, Dictionary<string, string> controlValueRequired)
        //updates the database with values in the data edit fields

        {

            string sql = string.Empty;

            try
            {
                Int32 selectedRowIdVal = WindowTasks.DataGridGetId(winDg);

                DataTable winSelectedRowDataTable = WindowDataOps.DbGetDataRow(windowMetaList, selectedRowIdVal, editStkPnl);

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

                        NpgsqlCommand listItemSaveSql = new NpgsqlCommand
                        {
                            CommandText = sql,
                            CommandType = CommandType.Text,
                            Connection = windowMetaList.ApplicationDb
                        };

                        listItemSaveSql.Parameters.AddWithValue("@Id", selectedRowIdVal);

                        windowMetaList.ApplicationDb.Open();
                        listItemSaveSql.ExecuteNonQuery();
                        windowMetaList.ApplicationDb.Close();
                    };


                }
                return true;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Save Record:" + ex.Message, sql);
                windowMetaList.ApplicationDb.Close();
                return false;
            };
        }

        public static void DbDeleteRecord(WindowMetaList windowMetaList, DataGrid winDg)

        //deletes the selected row from the database

        {

            string sql = string.Empty;
            try
            {
                Int32 selectedRowIdVal = WindowTasks.DataGridGetId(winDg);

                //Delete the selected row from db
                sql = "DELETE FROM " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " WHERE " + windowMetaList.TableKey + " = @Id";

                NpgsqlCommand delRowSql = new NpgsqlCommand
                {
                    CommandText = sql,
                    CommandType = CommandType.Text,
                    Connection = windowMetaList.ApplicationDb
                };

                delRowSql.Parameters.AddWithValue("@Id", selectedRowIdVal);

                windowMetaList.ApplicationDb.Open();
                delRowSql.ExecuteNonQuery();
                windowMetaList.ApplicationDb.Close();

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Delete Record:" + ex.Message, sql);
                windowMetaList.ApplicationDb.Close();
            };
        }

    }
}
