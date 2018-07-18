using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace dbRad
{
    public partial class WindowTasks
    {
        public static List<string> winMetadataList(string tabId)
        //Returns the list of metadata values for a window
        {
            SqlConnection ctrlDbCon = new SqlConnection(Config.appDb.ToString());
            List<string> listRange = new List<string>();

            //get the table string values
            SqlCommand getTab = new SqlCommand();

            getTab.CommandText =
              @"SELECT a.ApplicationName,
                       t.TableName,
                       t.TableLabel,
                       s.SchemaName,
                       s.SchemaLabel,
                       t.TableKey,
                       t.ApplicationTableId
                FROM control.metadata.Application a
                     INNER JOIN control.metadata.ApplicationSchema apps ON a.ApplicationId = apps.ApplicationId
                     INNER JOIN control.metadata.ApplicationTable t ON t.ApplicationSchemaId = apps.ApplicationSchemaId
                     INNER JOIN control.metadata.[Schema] s ON apps.SchemaId = s.SchemaId
                                WHERE ApplicationTableId = @tabId";

            getTab.CommandType = CommandType.Text;
            getTab.Parameters.AddWithValue("@tabId", tabId);
            getTab.Connection = ctrlDbCon;
            ctrlDbCon.Open();

            SqlDataReader getTabReader = getTab.ExecuteReader();
            getTabReader.Read();

            listRange.Add(getTabReader["ApplicationName"].ToString());
            listRange.Add(getTabReader["TableKey"].ToString());
            listRange.Add(getTabReader["TableName"].ToString());
            listRange.Add(getTabReader["TableLabel"].ToString());
            listRange.Add(getTabReader["SchemaName"].ToString());
            listRange.Add(getTabReader["SchemaLabel"].ToString());


            ctrlDbCon.Close();
            return listRange;

        }

        public static string dataGridGetId(DataGrid winDg)
        //Gets the Id of the selected grid row
        {
            string selectedRowIdVal;
            try
            {
                DataRowView drv = (DataRowView)winDg.SelectedValue;
                selectedRowIdVal = drv.Row.ItemArray[0].ToString();
                return selectedRowIdVal;
            }
            catch
            {
                selectedRowIdVal = null;
                return selectedRowIdVal;
            }
        }

        public static void winClose(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            Window w = Window.GetWindow(clicked);
            w.Close();
        }


        public static void appShutdown(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
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


        public static void DisplayError(Exception ex, string msg, string debug)
        {
            MessageBox.Show(msg + "\r\n\nDev Message\r\n" + debug + "\r\n\nException Message" + ex.Message + "\r\n\nStack Trace" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
