using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Npgsql;

namespace dbRad.Classes
{
    public partial class WindowTasks
    {
        public static WindowMetaList WinMetadataList(Int32 tabId)
        //Returns the list of metadata values for a window
        {
            NpgsqlConnection controlDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("control"));

            WindowMetaList metaList = new WindowMetaList
            {
                //Set the control connections
                ControlDb = controlDb
            };

            //get the table string values
            NpgsqlCommand getTab = new NpgsqlCommand
            {
                CommandText = ControlDatabaseSql.TableMetadata(),
                CommandType = CommandType.Text,
                Connection = controlDb
            };

            getTab.Parameters.AddWithValue("@tabId", tabId);
            controlDb.Open();

            NpgsqlDataReader getTabReader = getTab.ExecuteReader();
            getTabReader.Read();

            metaList.ApplicationName = getTabReader["application_name"].ToString();
            metaList.TableId = Convert.ToInt32(getTabReader["application_table_id"]);
            metaList.TableKey = getTabReader["table_key"].ToString();
            metaList.TableName = getTabReader["table_name"].ToString();
            metaList.TableLabel = getTabReader["table_label"].ToString();
            metaList.TableDml = getTabReader["table_dml"].ToString();
            metaList.TableOrderBy = getTabReader["table_order_by"].ToString();
            metaList.PageRowCount = getTabReader["page_row_count"].ToString();
            metaList.SchemaName = getTabReader["schema_name"].ToString();
            metaList.SchemaLabel = getTabReader["schema_label"].ToString();

            controlDb.Close();
            //set the application connection
            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(metaList.ApplicationName));
            metaList.ApplicationDb = applicationDb;
            metaList.Columns = WindowBuildUtils.PopulateColumnMetadata(metaList);

            //metaList.Columns.Add(new ColumMetadata { })


            return metaList;

        }

        public static Int32 DataGridGetId(DataGrid winDg)
        //Gets the Id of the selected grid row
        {
            Int32 selectedRowIdVal;
            try
            {
                DataRowView drv = (DataRowView)winDg.SelectedValue;
                selectedRowIdVal = Convert.ToInt32(drv.Row.ItemArray[0]);
                return selectedRowIdVal;
            }
            catch
            {
                selectedRowIdVal = 0;
                return selectedRowIdVal;
            }
        }

        public static void WinClose(object sender, RoutedEventArgs e)
        {
            Window w = new Window();

            switch (sender.GetType().Name)
            {
                case "Button":
                    Button buttonClicked = (Button)sender;
                    w = Window.GetWindow(buttonClicked);
                    break;

                case "MenuItem":
                    MenuItem menuItemClicked = (MenuItem)sender;
                    w = Window.GetWindow(menuItemClicked);
                    break;
            }
            w.Close();
        }

        public static void WinSetMode(String winMode, Window winNew, Button btnSave, Button btnNew, Button btnDelete, Button btnExit, Button btnClear, WindowMetaList windowMetaList, TextBox tbWinMode)
        //Sets the various mode for the winow
        {

            switch (winMode)

            {
                case "SELECT":
                    btnSave.IsEnabled = true;
                    btnNew.IsEnabled = true;
                    btnDelete.IsEnabled = false;
                    btnExit.IsEnabled = true;
                    btnClear.IsEnabled = true;
                    break;
                case "NEW":
                    btnSave.IsEnabled = true;
                    btnNew.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    btnExit.IsEnabled = true;
                    btnClear.IsEnabled = true;
                    break;
                case "EDIT":
                    btnSave.IsEnabled = true;
                    btnNew.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    btnExit.IsEnabled = true;
                    btnClear.IsEnabled = true;
                    break;
                case "CLEAR":
                    btnSave.IsEnabled = false;
                    btnNew.IsEnabled = true;
                    btnDelete.IsEnabled = false;
                    btnExit.IsEnabled = true;
                    btnClear.IsEnabled = true;
                    winMode = "SELECT";
                    break;
            }
            tbWinMode.Text = ApplicationEnviroment.ApplicationMessage(winMode);
            windowMetaList.WinMode = winMode;

        }

        public static void WinClearDataFields(Window winNew, StackPanel editStkPnl, StackPanel fltStkPnl, bool keepFilters, WindowMetaList windowMetaList, Dictionary<string, string> controlValues)
        //Clears the data edit fields

        {
            string filterList = windowMetaList.TableFilter;
            //winNew.FindResource("winFilter").ToString();
            //foreach (FrameworkElement element in editStkPnl.Children)
            foreach (var item in windowMetaList.Columns)
            {
                //string ctlType = element.GetType().Name;
                //switch (ctlType)
                switch (item.ColumnType)
                {
                    case "TEXT":
                    case "TEXTBLOCK":
                    case "ROWSOURCE":
                    case "FILTER":
                    case "ORDERBY":
                    case "NUM":
                        TextBox tb = (TextBox)editStkPnl.FindName(item.ColumnName);
                        switch (keepFilters)
                        {
                            case true:
                                if (filterList.Contains(tb.Name))
                                {
                                    tb.Text = item.ColumnValue;
                                }
                                else
                                {
                                    tb.Text = null;
                                }
                                break;

                            case false:
                                tb.Text = null;
                                break;

                        }
                        break;

                    case "COMBO":
                        ComboBox cb = (ComboBox)editStkPnl.FindName(item.ColumnName);
                        switch (keepFilters)
                        {
                            case true:

                                if (filterList.ToLower().Contains(cb.Name))
                                {
                                    cb.SelectedValue = item.ColumnValue.NullIfWhiteSpace();
                                }
                                else
                                {
                                    cb.SelectedValue = null;
                                }
                                break;

                            case false:
                                cb.SelectedValue = null;
                                break;

                        }
                        break;

                    case "DATE":
                        DatePicker dtp = (DatePicker)editStkPnl.FindName(item.ColumnName);
                        dtp.SelectedDate = null;
                        break;

                    case "CHK":
                        CheckBox chk = (CheckBox)editStkPnl.FindName(item.ColumnName);
                        chk.IsChecked = null;
                        break;

                };
            }
        }

        public static void WinSetControlDefaultValues(StackPanel editStkPnl, WindowMetaList windowMetaList)
        //Set control values to defaults
        {
            //MessageBox.Show(WindowTasks.ShowColumnList(windowMetaList));
            foreach (var item in windowMetaList.Columns)
            {
                if (item.ColumnName != "")
                {

                    switch (item.ColumnType)
                    {
                        case "TEXT":
                        case "TEXTBLOCK":
                        case "ROWSOURCE":
                        case "FILTER":
                        case "ORDERBY":
                        case "NUM":
                            TextBox tb = (TextBox)editStkPnl.FindName(item.ColumnName);
                            tb.Text = tb.Text.NullIfWhiteSpace() ?? item.ColumnDefaultValue;
                            break;

                        case "COMBO":
                            ComboBox cb = (ComboBox)editStkPnl.FindName(item.ColumnName);
                            cb.Text = cb.Text.NullIfWhiteSpace() ?? item.ColumnDefaultValue;
                            break;

                        case "DATE":
                            DatePicker dtp = (DatePicker)editStkPnl.FindName(item.ColumnName);
                            dtp.SelectedDate = dtp.SelectedDate ?? Convert.ToDateTime(item.ColumnDefaultValue);
                            break;

                        case "CHK":
                            CheckBox chk = (CheckBox)editStkPnl.FindName(item.ColumnName);
                            chk.IsChecked = Convert.ToBoolean(item.ColumnDefaultValue);
                            break;
                    }
                };
            }
        }
        public static void WinResetRecordSelector(TextBox tbSelectorText, TextBox tbOffset, TextBox tbFetch, WindowMetaList windowMetaList)
        {
            tbSelectorText.Text = "";
            tbOffset.Text = "0";
            tbFetch.Text = windowMetaList.PageRowCount;
        }

        public static void WinDataGridSelectRow(Int32 id, DataGrid winDg, WindowMetaList windowMetaList)
        //Selects the row in the data grid for the current id
        {
            int x = windowMetaList.GridSelectedIndex;
            try
            {
                object item = winDg.Items[x];
                winDg.SelectedItem = item;
                winDg.ScrollIntoView(item);
                winDg.UpdateLayout();
                DataGridRow row = (DataGridRow)winDg.ItemContainerGenerator.ContainerFromIndex(x);
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Selecting the DataGrid Row:", null);
            }
        }
        public static string ShowColumnList(WindowMetaList windowMetaList)
        {
            string columnValues = string.Empty;

            foreach (var item in windowMetaList.Columns)
            {
                columnValues += item.ColumnName + "; Type = " + item.ColumnType + "; Default = " + item.ColumnDefaultValue + "\n";
            }
            return columnValues;
        }
        public static void DisplayError(Exception ex, string msg, string debug)
        {
            MessageBox.Show(msg + "\r\n\nDev Message\r\n" + debug + "\r\n\nException Message" + ex.Message + "\r\n\nStack Trace" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static void DisplayMessage(string msg)
        {
            MessageBox.Show(msg, "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
