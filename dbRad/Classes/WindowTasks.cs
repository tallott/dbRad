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
            metaList.SchemaName = getTabReader["schema_name"].ToString();
            metaList.SchemaLabel = getTabReader["schema_label"].ToString();

            //set the application connection
            NpgsqlConnection applicationDb = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(metaList.ApplicationName));
            metaList.ApplicationDb = applicationDb;

            controlDb.Close();
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
            Button clicked = (Button)sender;
            Window w = Window.GetWindow(clicked);
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

        public static void ResetWinMain()
        {

            Window winMain = new dbRad.MainWindow();
            winMain.Show();
            foreach (Window window in App.Current.Windows)
            {
                if (window.Name != "winConfig")
                {

                    window.Close();
                    break;

                }
            }

        }

        public static void WinClearDataFields(Window winNew, StackPanel editStkPnl, StackPanel fltStkPnl, bool keepFilters, WindowMetaList windowMetaList, Dictionary<string, string> controlValues)
        //Clears the data edit fields

        {
            string filterList = windowMetaList.TableFilter;
            //winNew.FindResource("winFilter").ToString();
            foreach (FrameworkElement element in editStkPnl.Children)
            {
                string ctlType = element.GetType().Name;
                switch (ctlType)
                {
                    case "TextBox":
                        TextBox tb = (TextBox)editStkPnl.FindName(element.Name);
                        switch (keepFilters)
                        {
                            case true:
                                if (!filterList.Contains(tb.Name))
                                {
                                    tb.Text = null;
                                }

                                break;

                            case false:
                                tb.Text = null;
                                break;

                        }
                        break;

                    case "ComboBox":
                        ComboBox cb = (ComboBox)editStkPnl.FindName(element.Name);
                        switch (keepFilters)
                        {
                            case true:

                                if (!filterList.ToLower().Contains(cb.Name))
                                {
                                    cb.SelectedValue = null;
                                }

                                break;

                            case false:
                                cb.SelectedValue = null;
                                break;

                        }
                        break;

                    case "DatePicker":
                        DatePicker dtp = (DatePicker)editStkPnl.FindName(element.Name);
                        dtp.SelectedDate = null;
                        break;

                    case "CheckBox":
                        CheckBox chk = (CheckBox)editStkPnl.FindName(element.Name);
                        chk.IsChecked = null;
                        break;

                };
            }
        }

        public static void WinSetControlDefaultValues(StackPanel editStkPnl, Dictionary<string, string> controlValues)
        //Clears the list of window values used for filters
        {
            foreach (KeyValuePair<string, string> item in controlValues)
            {
                if (item.Value != "")
                {
                    //Determine the Type of control
                    object obj = editStkPnl.FindName(item.Key);

                    string ctlType = obj.GetType().Name;
                    //Use Type to work out how to process value;

                    switch (ctlType)
                    {
                        case "TextBox":
                            TextBox tb = (TextBox)editStkPnl.FindName(item.Key);
                            tb.Text = item.Value;
                            break;

                        case "ComboBox":
                            ComboBox cb = (ComboBox)editStkPnl.FindName(item.Key);
                            cb.Text = item.Value;
                            break;

                        case "DatePicker":
                            DatePicker dtp = (DatePicker)editStkPnl.FindName(item.Key);
                            dtp.SelectedDate = Convert.ToDateTime(item.Value);
                            break;

                        case "CheckBox":
                            CheckBox chk = (CheckBox)editStkPnl.FindName(item.Key);
                            chk.IsChecked = Convert.ToBoolean(item.Value);
                            break;
                    }
                };
            }
        }

        public static void WinResetRecordSelector(TextBox tbSelectorText, TextBox tbOffset, TextBox tbFetch)
        {
            tbSelectorText.Text = "";
            tbOffset.Text = "0";
            tbFetch.Text = "25";
        }

        public static void WinDataGridSelectRow(Int32 id, DataGrid winDg)
        //Selects the row in the data grid for the current id
        {
            winDg.UpdateLayout();
            try
            {
                for (int i = 0; i < winDg.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)winDg.ItemContainerGenerator.ContainerFromIndex(i);


                    if (winDg.Columns[0].GetCellContent(row) is TextBlock cellContent && cellContent.Text.Equals(id.ToString()))
                    {
                        object item = winDg.Items[i];
                        winDg.SelectedItem = item;
                        winDg.ScrollIntoView(item);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Problem Selecting the DataGrid Row:", null);
            }
        }

        public static void DisplayError(Exception ex, string msg, string debug)
        {
            MessageBox.Show(msg + "\r\n\nDev Message\r\n" + debug + "\r\n\nException Message" + ex.Message + "\r\n\nStack Trace" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
