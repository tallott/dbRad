using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dbRad
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


        public static void winClearControlDictionaryValues(Dictionary<string, string> controlValues)
        //Clears the list of window values used for filters
        {
            foreach (string key in controlValues.Keys.ToList())
            {
                controlValues[key] = "";
            }
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


        public static void winLoadFilterValues(StackPanel editStkPnl, string editColumn, string filterValue)
        {
            string colName = editColumn.Replace("$", "");
            object obj = editStkPnl.FindName(colName);
            string ctlType = obj.GetType().Name;

            //Use Type to work out how to process value;

            switch (ctlType)
            {
                case "TextBox":
                    TextBox tb = (TextBox)editStkPnl.FindName(colName);
                    tb.Text = filterValue;
                    break;

                case "ComboBox":
                    ComboBox cb = (ComboBox)editStkPnl.FindName(colName);
                    cb.SelectedValue = filterValue;
                    break;

                case "DatePicker":
                    DatePicker dtp = (DatePicker)editStkPnl.FindName(colName);
                    if (filterValue != "")
                    {
                        dtp.SelectedDate = Convert.ToDateTime(filterValue);
                    }
                    else if (filterValue == "")
                    {
                        dtp.SelectedDate = null;
                    }
                    break;

                case "CheckBox":
                    CheckBox chk = (CheckBox)editStkPnl.FindName(colName);
                    chk.IsChecked = Convert.ToBoolean(filterValue);
                    break;
            };
        }
    }
}
