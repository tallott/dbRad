using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Npgsql;

namespace dbRad.Classes
{
    public partial class WindowTasks
    {
        public static WindowMetaList winMetadataList(Int32 tabId)
        //Returns the list of metadata values for a window
        {
            NpgsqlConnection ctrlDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("Control"));

            WindowMetaList metaList = new WindowMetaList();

            //get the table string values
            NpgsqlCommand getTab = new NpgsqlCommand();

            getTab.CommandText =
              @"SELECT a.ApplicationName,
                       t.ApplicationTableId,
                       t.TableName,
                       t.TableLabel,
                       s.SchemaName,
                       s.SchemaLabel,
                       t.TableKey,
                       t.ApplicationTableId
                FROM metadata.Application a
                     INNER JOIN metadata.ApplicationSchema apps ON a.ApplicationId = apps.ApplicationId
                     INNER JOIN metadata.ApplicationTable t ON t.ApplicationSchemaId = apps.ApplicationSchemaId
                     INNER JOIN metadata.Schema s ON apps.SchemaId = s.SchemaId
                WHERE ApplicationTableId = @tabId";

            getTab.CommandType = CommandType.Text;
            getTab.Parameters.AddWithValue("@tabId", tabId);
            getTab.Connection = ctrlDbCon;
            ctrlDbCon.Open();

            NpgsqlDataReader getTabReader = getTab.ExecuteReader();
            getTabReader.Read();

            metaList.ApplicationName = getTabReader["ApplicationName"].ToString();
            metaList.TableId = Convert.ToInt32(getTabReader["ApplicationTableId"]);
            metaList.TableKey = getTabReader["TableKey"].ToString();
            metaList.TableName = getTabReader["TableName"].ToString();
            metaList.TableLabel = getTabReader["TableLabel"].ToString();
            metaList.SchemaName = getTabReader["SchemaName"].ToString();
            metaList.SchemaLabel = getTabReader["SchemaLabel"].ToString();

            ctrlDbCon.Close();
            return metaList;

        }

        public static Int32 dataGridGetId(DataGrid winDg)
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

        public static void winClose(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            Window w = Window.GetWindow(clicked);
            w.Close();
        }

        public static void winSetMode(String winMode, Window winNew, Button btnSave, Button btnNew, Button btnDelete, Button btnExit, Button btnClear)
        //Sets the various mode for the winow
        {
            winNew.Resources.Remove("winMode");
            winNew.Resources.Add("winMode", winMode);

            switch (winMode)

            {
                case "SELECT":
                    btnSave.Visibility = Visibility.Hidden;
                    btnNew.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Hidden;
                    btnExit.Visibility = Visibility.Visible;
                    btnClear.Visibility = Visibility.Visible;
                    break;
                case "NEW":
                    btnSave.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Hidden;
                    btnDelete.Visibility = Visibility.Hidden;
                    btnExit.Visibility = Visibility.Visible;
                    btnClear.Visibility = Visibility.Visible;
                    break;
                case "EDIT":
                    btnSave.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                    btnExit.Visibility = Visibility.Visible;
                    btnClear.Visibility = Visibility.Visible;
                    break;
            }

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

        public static void winClearDataFields(Window winNew, StackPanel editStkPnl, StackPanel fltStkPnl, bool keepFilters)
        //Clears the data edit fields

        {
            string filterList = winNew.FindResource("winFilter").ToString();
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

        public static void winSetControlDefaultValues(StackPanel editStkPnl, Dictionary<string, string> controlValues)
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

        public static void winResetRecordSelector(TextBox tbSelectorText, TextBox tbOffset, TextBox tbFetch)
        {
            tbSelectorText.Text = "";
            tbOffset.Text = "0";
            tbFetch.Text = "25";
        }

        public static void winDataGridSelectRow(Int32 id, DataGrid winDg)
        //Selects the row in the data grid for the current id
        {
            winDg.UpdateLayout();
            try
            {
                for (int i = 0; i < winDg.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)winDg.ItemContainerGenerator.ContainerFromIndex(i);
                    TextBlock cellContent = winDg.Columns[0].GetCellContent(row) as TextBlock;


                    if (cellContent != null && cellContent.Text.Equals(id.ToString()))
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
