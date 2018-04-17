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
    public partial class WindowTasks
    {
      public  static List<string> winMetadataList(string tabId)
        //Returns the list of metadata values for a window
        {
            SqlConnection ctrlDbCon = new SqlConnection(Config.controlDb.ToString());
            List<string> listRange = new List<string>();
            X//Add additional list item for user permissions
            //get the table string values
            SqlCommand getTab = new SqlCommand();
            getTab.CommandText = "SELECT t.TableName, t.TableLabel, s.SchemaName, t.TableKey FROM ApplicationTable t INNER JOIN ApplicationSchema s ON t.ApplicationSchemaId  = s.ApplicationSchemaId WHERE ApplicationTableId = @tabId";
            getTab.CommandType = CommandType.Text;
            getTab.Parameters.AddWithValue("@tabId", tabId);
            getTab.Connection = ctrlDbCon;
            ctrlDbCon.Open();

            SqlDataReader getTabReader = getTab.ExecuteReader();
            getTabReader.Read();

            listRange.Add(getTabReader["TableName"].ToString());
            listRange.Add(getTabReader["TableLabel"].ToString());
            listRange.Add(getTabReader["SchemaName"].ToString());
            listRange.Add(getTabReader["TableKey"].ToString());

            ctrlDbCon.Close();
            return listRange;

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
    }
}
