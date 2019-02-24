using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using Npgsql;

namespace dbRad.Classes
{
    class WindowDataOps
    {
        public static string SubstituteWindowParameters(String targetTxt, Dictionary<string, string> columnValues)
        //Repalces parameter values with control values from editStkPnl
        {
            string x = columnValues.Count.ToString();
            foreach (string key in columnValues.Keys)
            {
                string s = ("~" + key + "~").ToLower();

                string r;
                string columnValue = columnValues[key];

                //Value supplied or not
                if ((columnValue ?? "") != "") //supplied
                {
                    r = columnValue;
                }
                else //Not supplied
                {
                    r = "0";
                }
                //replace the |COLUMN_NAME| parameter with the supplied value
                if (targetTxt.ToLower().Contains(s))
                {
                    targetTxt = targetTxt.ToLower().Replace(s, r);
                }
            }



            return targetTxt;
        }

        public static void winLoadDataRow(StackPanel editStkPnl, DataTable winSelectedRowDataTable, Dictionary<string, string> controlValues)
        //Loads the data editing UI with the values from the row in winSelectedRowDataTable 
        {
            //Loop the Row (Filtered by @Id) and columns of the underlying dataset
            string rowCol = null;
            string columnName = null;
            try
            {

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {

                    foreach (DataColumn col in winSelectedRowDataTable.Columns)
                    {
                        //Set the value of the control col.Name in the window to the value returned by row[col]
                        rowCol = row[col].ToString();
                        columnName = col.ColumnName;
                        //Determine the Type of control
                        object obj = editStkPnl.FindName(columnName);

                        string ctlType = obj.GetType().Name;
                        //Use Type to work out how to process value;

                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(columnName);
                                tb.Text = rowCol;
                                break;

                            case "ComboBox":
                                ComboBox cb = (ComboBox)editStkPnl.FindName(columnName);
                                if (rowCol != "")
                                {
                                    cb.SelectedValue = rowCol;
                                    //We set this here because there is no change event we can trigger on a combo box
                                    winGetControlValue(cb, controlValues);
                                }
                                else if (rowCol == "")
                                {
                                    cb.SelectedValue = null;
                                }
                                break;

                            case "DatePicker":
                                DatePicker dtp = (DatePicker)editStkPnl.FindName(columnName);
                                if (rowCol != "")
                                {
                                    dtp.SelectedDate = Convert.ToDateTime(row[col]);
                                }
                                else if (rowCol == "")
                                {
                                    dtp.SelectedDate = null;
                                }
                                break;

                            case "CheckBox":
                                CheckBox chk = (CheckBox)editStkPnl.FindName(columnName);
                                chk.IsChecked = Convert.ToBoolean(row[col]);
                                break;
                        };
                    }
                }
            }

            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Loading the data row:", columnName + ":" + rowCol);
            }


        }

        public static void winGetControlValue(ComboBox cb, Dictionary<string, string> controlValues)

        {
            controlValues[cb.Name] = cb.SelectedValue.ToString();
        }

        public static void winGetControlValue(TextBox tb, Dictionary<string, string> controlValues)

        {
            controlValues[tb.Name] = "'" + tb.Text + "'";
        }

        public static void winGetControlValue(CheckBox cb, Dictionary<string, string> controlValues)

        {
            controlValues[cb.Name] = cb.IsChecked.ToString();
        }

        public static void winGetControlValue(DatePicker dtp, Dictionary<string, string> controlValues)

        {
            controlValues[dtp.Name] = "'" + dtp.Text + "'";
        }

        public static void winDataGridClicked(WindowMetaList windowMetaList, DataGrid winDg,Int32 selectedRowIdVal, StackPanel editStkPnl, Dictionary<string, string> controlValues)
        //gets the id of the row selected and loads the edit fileds with the database values
        {
            if (selectedRowIdVal == 0)
            {
                selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
            }
            
            try
            {

                if (selectedRowIdVal != 0)
                {
                    DataTable winSelectedRowDataTable = dbGetDataRow(windowMetaList, selectedRowIdVal, editStkPnl);
                    WindowDataOps.winLoadDataRow(editStkPnl, winSelectedRowDataTable, controlValues);
                    winDg.UpdateLayout();
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Loading Data Grid:", null);
            }
        }

        public static DataTable dbGetDataRow(WindowMetaList windowMetaList, Int32 id, StackPanel editStkPnl)
        //Loads a single row from the database into a table for the record for the selected ID
        {
 
            string sql = "SELECT * FROM " + windowMetaList.SchemaName + "." + windowMetaList.TableName + " WHERE " + windowMetaList.TableKey + " = @Id";

            DataTable winSelectedRowDataTable = new DataTable();

            NpgsqlCommand winSelectedRowSql = new NpgsqlCommand();
            winSelectedRowSql.CommandText = sql;
            winSelectedRowSql.Parameters.AddWithValue("@Id", id);
            winSelectedRowSql.CommandType = CommandType.Text;
            winSelectedRowSql.Connection = windowMetaList.applicationDb;

            windowMetaList.applicationDb.Open();
            try
            {
                NpgsqlDataAdapter winDa = new NpgsqlDataAdapter(winSelectedRowSql);
                winDa.Fill(winSelectedRowDataTable);
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Loading data grid row:" + ex.Message, sql);
                windowMetaList.applicationDb.Close();
            }

            windowMetaList.applicationDb.Close();
            return winSelectedRowDataTable;

        }

        public static void winClearControlDictionaryValues(Dictionary<string, string> controlValues)
        //Clears the list of window values used for filters
        {
            foreach (string key in controlValues.Keys.ToList())
            {
                controlValues[key] = "";
            }
        }

        public static string winDataGridGetBaseSql(string sqlpart, Int32 sqlParam, WindowMetaList windowMetaList)
        //Single row to return user defined DML SQL for DataGrid
        {

            NpgsqlCommand getTabSql = new NpgsqlCommand();

            getTabSql.CommandText = sqlpart;
            getTabSql.Parameters.AddWithValue("@sqlparam", sqlParam);
            getTabSql.CommandType = CommandType.Text;
            getTabSql.Connection = windowMetaList.controlDb;


            windowMetaList.controlDb.Open();

            //Run the SQL cmd to return the base SQL that fills DataGrid

            string sqlTxt = Convert.ToString(getTabSql.ExecuteScalar());
            windowMetaList.controlDb.Close();
            return sqlTxt;
        }


        public static DataTable winPopulateCombo(ComboBox cb, WindowMetaList windowMetaList, string colname, StackPanel editStkPnl, Dictionary<string, string> controlValues, DataGrid winDg)
        {
            NpgsqlCommand getColList = new NpgsqlCommand();
            NpgsqlCommand getComboRows = new NpgsqlCommand();

            DataTable comboDataTable = new DataTable();

            string controlName;
            string controlLabel;
            string controlRowSource;
            string controlFilter;
            string controlOrderBy;
            string controlType;
            string controlEnabled;
            string controlDefaultvalue;

            getColList.CommandText = ControlDatabaseSql.ColumnMetadataForColumn();

            getColList.Parameters.AddWithValue("@applicationTableId", windowMetaList.TableId);
            getColList.Parameters.AddWithValue("@colname", colname);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = windowMetaList.controlDb;
            try
            {
                windowMetaList.controlDb.Open();
                {
                    NpgsqlDataReader getColListReader = getColList.ExecuteReader();
                    getColListReader.Read();
                    controlName = getColListReader["column_name"].ToString();
                    controlLabel = getColListReader["column_label"].ToString();
                    controlRowSource = getColListReader["row_source"].ToString();
                    controlFilter = getColListReader["filter"].ToString();
                    controlOrderBy = getColListReader["order_by"].ToString();
                    controlType = getColListReader["window_control_type"].ToString();
                    controlEnabled = getColListReader["window_control_enabled"].ToString();
                    controlDefaultvalue = getColListReader["column_default_value"].ToString();
                }
                windowMetaList.controlDb.Close();

                if (controlOrderBy == string.Empty)
                    controlOrderBy = "\nORDER BY 1";
                else
                    controlOrderBy = "\nORDER BY " + controlOrderBy;

                controlRowSource += controlOrderBy;
                controlRowSource = WindowDataOps.SubstituteWindowParameters(controlRowSource, controlValues);

                getComboRows.CommandText = controlRowSource;
                getComboRows.CommandType = CommandType.Text;
                getComboRows.Connection = windowMetaList.applicationDb;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR Reading Data:" + ex.Message, getColList.CommandText);
                windowMetaList.controlDb.Close();
            }
            try
            {
                windowMetaList.applicationDb.Open();
                {
                    NpgsqlDataAdapter comboAdapter = new NpgsqlDataAdapter(getComboRows);

                    comboAdapter.Fill(comboDataTable);
                    cb.ItemsSource = comboDataTable.DefaultView;
                    cb.DisplayMemberPath = comboDataTable.Columns["display_member"].ToString();
                    cb.SelectedValuePath = comboDataTable.Columns["value_member"].ToString();
                }
                windowMetaList.applicationDb.Close();
                return comboDataTable;
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR Filling Combo:" + ex.Message, getColList.CommandText);
                windowMetaList.applicationDb.Close();
                return comboDataTable;
            }
        }

    }
}
