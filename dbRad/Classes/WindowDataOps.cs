﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dbRad.Classes
{
    class WindowDataOps
    {
        public static string SubstituteWindowParameters(StackPanel editStkPnl, String targetTxt, Dictionary<string, string> columnValues)
        //Repalces parameter values with control values from editStkPnl
        {
            string x = columnValues.Count.ToString();
            foreach (string key in columnValues.Keys)
            {
                string s = "$" + key + "$";
                string r;
                string columnValue = columnValues[key];

                //Value supplied or not
                if ((columnValue ?? "") != "") //supplied
                {
                    r = columnValue;

                }
                else //Not supplied
                {
                    r = "''";
                }
                //replace the $COLUMN_NAME$ parameter with the supplied value
                if (targetTxt.Contains(s))
                {
                    targetTxt = targetTxt.Replace(s, r);

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

        public static void winDataGridClicked(String applicationTableId, DataGrid winDg, StackPanel editStkPnl, Dictionary<string, string> controlValues)
        //gets the id of the row selected and loads the edit fileds with the database values
        {
            string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
            try
            {

                if (selectedRowIdVal != null)
                {
                    DataTable winSelectedRowDataTable = dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);
                    WindowDataOps.winLoadDataRow(editStkPnl, winSelectedRowDataTable, controlValues);
                    winDg.UpdateLayout();
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Loading Data Grid:", null);
            }
        }

        public static DataTable dbGetDataRow(string applicationTableId, string id, StackPanel editStkPnl)
        //Loads a single row from the database into a table for the record for the selected ID
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());

            string applicationName = WindowTasks.winMetadataList(applicationTableId).ApplicationName;
            string tableKey = WindowTasks.winMetadataList(applicationTableId).TableKey;
            string tableName = WindowTasks.winMetadataList(applicationTableId).TableName;
            string tableLabel = WindowTasks.winMetadataList(applicationTableId).TableLabel;
            string schemaName = WindowTasks.winMetadataList(applicationTableId).SchemaName;
            string schemaLabel = WindowTasks.winMetadataList(applicationTableId).SchemaLabel;

            string sql = "SELECT * FROM [" + applicationName + "].[" + schemaName + "].[" + tableName + "] WHERE " + tableKey + " = @Id";

            DataTable winSelectedRowDataTable = new DataTable();

            SqlCommand winSelectedRowSql = new SqlCommand();
            winSelectedRowSql.CommandText = sql;
            winSelectedRowSql.Parameters.AddWithValue("@Id", id);
            winSelectedRowSql.CommandType = CommandType.Text;
            winSelectedRowSql.Connection = appDbCon;

            appDbCon.Open();
            try
            {
                SqlDataAdapter winDa = new SqlDataAdapter(winSelectedRowSql);
                winDa.Fill(winSelectedRowDataTable);
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Loading data grid row:" + ex.Message, sql);
                appDbCon.Close();
            }

            appDbCon.Close();
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

        public static string winDataGridGetBaseSql(string sqlpart, string sqlParam)
        //Single row to return user defined DML SQL for DataGrid
        {

            SqlConnection controlDbCon = new SqlConnection(Config.appDb.ToString());

            SqlCommand getTabSql = new SqlCommand();

            getTabSql.CommandText = sqlpart;
            getTabSql.Parameters.AddWithValue("@sqlparam", sqlParam);
            getTabSql.CommandType = CommandType.Text;
            getTabSql.Connection = controlDbCon;

            controlDbCon.Open();

            //Run the SQL cmd to return the base SQL that fills DataGrid
            string sqlTxt = Convert.ToString(getTabSql.ExecuteScalar());

            controlDbCon.Close();
            return sqlTxt;
        }


        public static DataTable winPopulateCombo(ComboBox cb, string applicationTableId, string colname, SqlConnection ctrlDbCon, SqlConnection appDbCon, StackPanel editStkPnl, Dictionary<string, string> controlValues, DataGrid winDg)
        {
            SqlCommand getColList = new SqlCommand();
            SqlCommand getComboRows = new SqlCommand();

            DataTable comboDataTable = new DataTable();

            string controlName;
            string controlLabel;
            string controlRowSource;
            string controlFilter;
            string controlOrderBy;
            string controlType;
            string controlEnabled;

            //string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
            //string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];

            getColList.CommandText =
                  @"SELECT c.ColumnName,
                           ISNULL(c.ColumnLable, c.ColumnName) AS ColumnLabel,
                           c.RowSource,
                           c.Filter,
                           c.OrderBy,
                           ct.WindowControlType,
                           c.WindowControlEnabled
                    FROM control.metadata.ApplicationColumn c
                         INNER JOIN control.metadata.WindowControlType ct ON c.WindowControlTypeId = ct.WindowControlTypeId
                    WHERE c.ApplicationTableId = @applicationTableId
                    AND c.ColumnName = @colname
                    ORDER BY c.WindowLayoutOrder";

            getColList.Parameters.AddWithValue("@applicationTableId", applicationTableId);
            getColList.Parameters.AddWithValue("@colname", colname);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = ctrlDbCon;

            ctrlDbCon.Open();
            {
                SqlDataReader getColListReader = getColList.ExecuteReader();
                getColListReader.Read();
                controlName = getColListReader["ColumnName"].ToString();
                controlLabel = getColListReader["ColumnLabel"].ToString();
                controlRowSource = getColListReader["RowSource"].ToString();
                controlFilter = getColListReader["Filter"].ToString();
                controlOrderBy = getColListReader["OrderBy"].ToString();
                controlType = getColListReader["WindowControlType"].ToString();
                controlEnabled = getColListReader["WindowControlEnabled"].ToString();
            }
            ctrlDbCon.Close();

            if (controlOrderBy == string.Empty)
                controlOrderBy = "\nORDER BY 1";
            else
                controlOrderBy = "\nORDER BY " + controlOrderBy;

            controlRowSource += controlOrderBy;
            controlRowSource = WindowDataOps.SubstituteWindowParameters(editStkPnl, controlRowSource, controlValues);

            getComboRows.CommandText = controlRowSource;
            getComboRows.CommandType = CommandType.Text;
            getComboRows.Connection = appDbCon;

            appDbCon.Open();
            {
                SqlDataAdapter comboAdapter = new SqlDataAdapter(getComboRows);

                comboAdapter.Fill(comboDataTable);
                cb.ItemsSource = comboDataTable.DefaultView;
                cb.DisplayMemberPath = comboDataTable.Columns["displayMember"].ToString();
                cb.SelectedValuePath = comboDataTable.Columns["valueMember"].ToString();

            }
            appDbCon.Close();
            return comboDataTable;
        }

    }
}