using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace dbRad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            appStartup();


        }
        private void appStartup()
        {
            if (File.Exists(Config.userFilePath))
            {
                Config.applicationUser = Filetasks.ReadFromXmlFile<User>(Config.userFilePath);
            }
            if (File.Exists(Config.appDbFilePath))
            {
                Config.appDb = Filetasks.ReadFromXmlFile<Connections>(Config.appDbFilePath);
            }
            //if (File.Exists(Config.applicationDbFilePath))
            //{
            //    Config.applicationlDb = Filetasks.ReadFromXmlFile<Connections>(Config.applicationDbFilePath);
            //}
            if (Config.appDb.HostName == string.Empty || Config.applicationUser.UserName == string.Empty)
            {
                Window config = new Config();
                config.Show();
            }
            else
            {
                mainWinBuild();
            }

        }

        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        //Makes sure users can only enter numerics
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void showConfig(object sender, EventArgs e)
        //Open config window
        {
            Window winConfig = new Config();
            winConfig.Show();
        }

        private void dbGetDataGridRows(Window winNew, String applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> columnValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());

            DataTable winDt = new DataTable();

            string sqlPart;
            string sqlParam = applicationTableId;

            //Single row to return user defined DML SQL for DataGrid
            sqlPart =
              @"SELECT TOP 1 Dml
                FROM control.metadata.ApplicationTable
                WHERE ApplicationTableId = @sqlParam";

            string sqlTxt = dataGridGetBaseSql(sqlPart, sqlParam);

            //Append filter where clause to the end of DML
            if (selectedFilter == 0) //Default filter selected
            {
                sqlPart =
                  @"SELECT FilterDefinition
                    FROM control.metadata.ApplicationFilter
                    WHERE ApplicationTableId = @sqlparam
                            AND SortOrder = 1";
            }
            else //Custom filter selected
            {
                sqlParam = selectedFilter.ToString();
                sqlPart =
                  @"SELECT FilterDefinition
                    FROM control.metadata.ApplicationFilter
                    WHERE ApplicationFilterId = @sqlparam";
            }

            string fltTxt = dataGridGetBaseSql(sqlPart, sqlParam);
            //string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
            //string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            winNew.Resources.Remove("winFilter");
            winNew.Resources.Add("winFilter", fltTxt);
            //Build where clause with replacement values for $COLUMN_NAME$ parameters  
            fltTxt = WindowDataOps.SubstituteWindowParameters(editStkPnl, fltTxt, columnValues);
            sqlTxt = sqlTxt + " WHERE " + fltTxt;
            string sqlCountText = sqlTxt;
            sqlTxt = sqlTxt + " ORDER BY 1 OFFSET " + tbOffset.Text + " ROWS FETCH NEXT " + tbFetch.Text + " ROWS ONLY";

            try
            {
                {
                    appDbCon.Open();
                    {
                        //Run the SQL cmd to return SQL that fills DataGrid
                        SqlCommand execTabSql = appDbCon.CreateCommand();
                        execTabSql.CommandText = sqlTxt;

                        //Create an adapter and fill the grid using sql and adapater
                        SqlDataAdapter winDa = new SqlDataAdapter(execTabSql);
                        winDa.Fill(winDt);
                        winDg.ItemsSource = winDt.DefaultView;

                        //set the page counter
                        string tabSchema = WindowTasks.winMetadataList(applicationTableId)[4];
                        string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
                        int rowCount = 0;

                        int chrStart = sqlCountText.IndexOf("SELECT") + 6;
                        int chrEnd = sqlCountText.IndexOf("FROM");

                        sqlTxt = sqlCountText.Substring(0, chrStart) + "  COUNT(*) " + sqlCountText.Substring(chrEnd);
                        SqlCommand countRows = new SqlCommand(sqlTxt, appDbCon);
                        rowCount = (int)countRows.ExecuteScalar();
                        int pageSize = Convert.ToInt32(tbFetch.Text);
                        int offSet = Convert.ToInt32(tbOffset.Text);

                        string pageCount = Convert.ToString((rowCount / pageSize) + 1);
                        string pageNumber = Convert.ToString((offSet / pageSize) + 1);


                        tbSelectorText.Text = "Page " + pageNumber + " of " + pageCount;
                        //Define the grid columns

                        appDbCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "ERROR in Filter SQL:" + ex.Message, sqlTxt);
                appDbCon.Close();
            }
        }

        private string dataGridGetBaseSql(string sqlpart, string sqlParam)
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

        private void dataGridSelectRow(string id, DataGrid winDg)
        //Selects the row in the data grid for the current id
        {
            winDg.UpdateLayout();
            try
            {
                for (int i = 0; i < winDg.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)winDg.ItemContainerGenerator.ContainerFromIndex(i);
                    TextBlock cellContent = winDg.Columns[0].GetCellContent(row) as TextBlock;
                    if (cellContent != null && cellContent.Text.Equals(id))
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



        private void dataGridClicked(String applicationTableId, DataGrid winDg, StackPanel editStkPnl, Dictionary<string, string> controlValues)
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

        private void winResetRecordSelector(TextBox tbSelectorText, TextBox tbOffset, TextBox tbFetch)
        {
            tbSelectorText.Text = "";
            tbOffset.Text = "0";
            tbFetch.Text = "25";
        }

        private void winClearDataFields(Window winNew, StackPanel editStkPnl, StackPanel fltStkPnl, bool keepFilters)
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

                                if (!filterList.Contains(cb.Name))
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

        private DataTable dbGetDataRow(string applicationTableId, string id, StackPanel editStkPnl)
        //Loads a single row from the database into a table for the record for the selected ID
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());

            string appName = WindowTasks.winMetadataList(applicationTableId)[0];
            string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
            string tabLabel = WindowTasks.winMetadataList(applicationTableId)[3];
            string schName = WindowTasks.winMetadataList(applicationTableId)[4];
            string schLabel = WindowTasks.winMetadataList(applicationTableId)[5];

            string sql = "SELECT * FROM [" + appName + "].[" + schName + "].[" + tabName + "] WHERE " + tabKey + " = @Id";

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

        private void dbCreateRecord(Window winNew, String applicationTableId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Creates a new record in the db
        {
            string appName = WindowTasks.winMetadataList(applicationTableId)[0];
            string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
            string tabLabel = WindowTasks.winMetadataList(applicationTableId)[3];
            string schName = WindowTasks.winMetadataList(applicationTableId)[4];
            string schLabel = WindowTasks.winMetadataList(applicationTableId)[5];


            List<string> columns = new List<string>();
            List<string> columnUpdates = new List<string>();

            string sql = string.Empty;
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());
            try
            {
                foreach (FrameworkElement element in editStkPnl.Children)
                {
                    if (element.Name != tabKey)
                    {

                        string ctlType = element.GetType().Name;
                        // MessageBox.Show(element.Name);
                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(element.Name);
                                columns.Add(element.Name);
                                columnUpdates.Add("'" + tb.Text + "'");
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
                                columnUpdates.Add(((bool)chk.IsChecked ? 1 : 0).ToString());
                                break;
                        };

                    }
                }
                string csvColumns = "(" + String.Join(",", columns) + ")";
                string csvColumnUpdates = " VALUES(" + String.Join(",", columnUpdates) + ")";
                sql = "INSERT INTO [" + appName + "].[" + schName + "].[" + tabName + "]" + csvColumns + csvColumnUpdates;

                SqlCommand dbCreateRecordSql = new SqlCommand();
                dbCreateRecordSql.CommandText = sql;
                dbCreateRecordSql.CommandType = CommandType.Text;
                dbCreateRecordSql.Connection = appDbCon;

                appDbCon.Open();
                dbCreateRecordSql.ExecuteNonQuery();
                appDbCon.Close();

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Insert Record:" + ex.Message, sql);
                appDbCon.Close();
            }
        }

        private void dbUpdateRecord(Window winNew, String applicationTableId, DataGrid winDg, StackPanel editStkPnl, StackPanel fltStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //updates the database with values in the data edit fields
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());

            string appName = WindowTasks.winMetadataList(applicationTableId)[0];
            string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
            string tabLabel = WindowTasks.winMetadataList(applicationTableId)[3];
            string schName = WindowTasks.winMetadataList(applicationTableId)[4];
            string schLabel = WindowTasks.winMetadataList(applicationTableId)[5];

            string sql = string.Empty;

            try
            {
                string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                DataTable winSelectedRowDataTable = dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);

                Boolean isDirty = false;

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    sql = "UPDATE [" + appName + "].[" + schName + "].[" + tabName + "] SET ";
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
                                    sql = sql + col.ColumnName + " = '" + tb.Text + "', ";
                                    isDirty = true;
                                };
                                break;

                            case "ComboBox":

                                ComboBox cb = (ComboBox)editStkPnl.FindName(col.ColumnName);
                                if (cb.SelectedValue.ToString() != row[col].ToString())
                                {
                                    sql = sql + col.ColumnName + " = " + cb.SelectedValue + ", ";
                                    isDirty = true;
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
                                    sql = sql + col.ColumnName + " = '" + Convert.ToBoolean(chk.IsChecked) + "', ";
                                    isDirty = true;
                                }
                                    ;
                                break;
                        };

                    }
                    if (isDirty)
                    { //Update the selected record in database

                        sql = sql.Trim(',', ' ') + " WHERE " + tabKey + " = @Id";

                        SqlCommand listItemSaveSql = new SqlCommand();
                        listItemSaveSql.CommandText = sql;
                        listItemSaveSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                        listItemSaveSql.CommandType = CommandType.Text;
                        listItemSaveSql.Connection = appDbCon;

                        appDbCon.Open();
                        listItemSaveSql.ExecuteNonQuery();
                        appDbCon.Close();

                        dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                        TextBox tabIdCol = (TextBox)editStkPnl.FindName(tabKey);
                        selectedRowIdVal = tabIdCol.Text;

                        dataGridSelectRow(selectedRowIdVal, winDg);

                    };


                }

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Save Record:" + ex.Message, sql);
                appDbCon.Close();
            };
        }

        private void dbDeleteRecord(Window winNew, String applicationTableId, StackPanel fltStkPnl, DataGrid winDg, StackPanel editStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //deletes the selected row from the database
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());

            string appName = WindowTasks.winMetadataList(applicationTableId)[0];
            string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
            string tabLabel = WindowTasks.winMetadataList(applicationTableId)[3];
            string schName = WindowTasks.winMetadataList(applicationTableId)[4];
            string schLabel = WindowTasks.winMetadataList(applicationTableId)[5];

            string sql = string.Empty;
            try
            {
                string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);
                DataTable winSelectedRowDataTable = dbGetDataRow(applicationTableId, selectedRowIdVal, editStkPnl);
                //Delete the selected row from db

                sql = "DELETE FROM [" + appName + "].[" + schName + "].[" + tabName + "] WHERE " + tabKey + " = @Id";

                SqlCommand delRowSql = new SqlCommand();
                delRowSql.CommandText = sql;
                delRowSql.Parameters.AddWithValue("@Id", selectedRowIdVal);
                delRowSql.CommandType = CommandType.Text;
                delRowSql.Connection = appDbCon;

                appDbCon.Open();
                delRowSql.ExecuteNonQuery();
                appDbCon.Close();

                winClearDataFields(winNew, editStkPnl, fltStkPnl, false);

                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Delete Record:" + ex.Message, sql);
                appDbCon.Close();
            };
        }

        private void mainWinBuild()

        //Builds the main window for the appDbName
        {
            SqlConnection ctrlAppDbCon = new SqlConnection(Config.appDb.ToString());
            SqlConnection ctrlSchDbCon = new SqlConnection(Config.appDb.ToString());
            SqlConnection ctrlTabDbCon = new SqlConnection(Config.appDb.ToString());

            //Get configured Application database
            string appDbName;
            string userName = Config.applicationUser.UserName;
            string userPassword = Config.applicationUser.UserPassword;
            string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.Title = "User = " + userName + " | Version:" + version;

            //Build Main Menu

            //A File Menu
            MenuItem fileMenu = new MenuItem();
            fileMenu.Header = "File";
            this.menu.Items.Add(fileMenu);

            //Item 1 Exit
            MenuItem fileExitItem = new MenuItem();
            fileExitItem.Header = "Exit";
            fileExitItem.Click += new RoutedEventHandler(WindowTasks.appShutdown);
            fileMenu.Items.Add(fileExitItem);

            //A config Menu
            MenuItem configMenu = new MenuItem();
            configMenu.Header = "Config";
            this.menu.Items.Add(configMenu);

            //Item 1 Database
            MenuItem databaseConfigItem = new MenuItem();
            databaseConfigItem.Header = "Settings";
            databaseConfigItem.Click += new RoutedEventHandler(showConfig);
            configMenu.Items.Add(databaseConfigItem);

            //An Open Menu
            MenuItem openMenu = new MenuItem();
            openMenu.Header = "Open";
            this.menu.Items.Add(openMenu);

            //Open Menu Items are Dynamicly populated with application/schema/table names in control tables;
            try
            {
                SqlCommand getAppList = new SqlCommand();
                getAppList.CommandText =
                      @"SELECT DISTINCT
                           ApplicationId,
                           ApplicationName
                    FROM control.directory.UserObjectPermisions
                    WHERE UserName = @UserName
                          AND UserPassword = @UserPassword
                    ORDER BY ApplicationName";

                getAppList.CommandType = CommandType.Text;
                getAppList.Parameters.AddWithValue("@UserName", userName);
                getAppList.Parameters.AddWithValue("@UserPassword", userPassword);

                getAppList.Connection = ctrlAppDbCon;

                ctrlAppDbCon.Open();

                SqlDataReader appReader = getAppList.ExecuteReader();
                while (appReader.Read())
                {
                    string appId = appReader["ApplicationId"].ToString();
                    appDbName = appReader["ApplicationName"].ToString();
                    MenuItem AppOpen = new MenuItem();
                    AppOpen.Header = appDbName;
                    openMenu.Items.Add(AppOpen);
                    {
                        SqlCommand getSchList = new SqlCommand();
                        getSchList.CommandText =
                              @"SELECT DISTINCT ApplicationSchemaId,
                           SchemaLabel
                    FROM control.directory.UserObjectPermisions
                    WHERE ApplicationName = @appDbName
                      AND UserName = @UserName
                      AND UserPassword = @UserPassword
                    ORDER BY SchemaLabel";

                        getAppList.Connection = ctrlAppDbCon;
                        ctrlSchDbCon.Open();
                        getSchList.CommandType = CommandType.Text;
                        getSchList.Parameters.AddWithValue("@appDbName", appDbName);
                        getSchList.Parameters.AddWithValue("@UserName", userName);
                        getSchList.Parameters.AddWithValue("@UserPassword", userPassword);
                        getSchList.Connection = ctrlSchDbCon;
                        {
                            SqlDataReader schReader = getSchList.ExecuteReader();

                            while (schReader.Read())
                            {
                                string schId = schReader["ApplicationSchemaId"].ToString();
                                string schName = schReader["SchemaLabel"].ToString();
                                MenuItem schemaOpen = new MenuItem();
                                schemaOpen.Header = schName;
                                AppOpen.Items.Add(schemaOpen);
                                SqlCommand getTabList = new SqlCommand();

                                getTabList.CommandText =
                                      @"SELECT DISTINCT ApplicationTableId,
                                                TableLabel
                                        FROM control.directory.UserObjectPermisions
                                        WHERE ApplicationName = @appDbName
                                          AND UserName = @UserName
                                          AND UserPassword = @UserPassword
                                          AND ApplicationSchemaId = @ApplicationSchemaId
                                        ORDER BY TableLabel";

                                getTabList.CommandType = CommandType.Text;

                                getTabList.Connection = ctrlTabDbCon;
                                ctrlTabDbCon.Open();
                                getTabList.Parameters.AddWithValue("@appDbName", appDbName);
                                getTabList.Parameters.AddWithValue("@UserName", userName);
                                getTabList.Parameters.AddWithValue("@UserPassword", userPassword);
                                getTabList.Parameters.AddWithValue("@ApplicationSchemaId", schId);
                                SqlDataReader tabReader = getTabList.ExecuteReader();
                                while (tabReader.Read())
                                {
                                    string applicationTableId = tabReader["ApplicationTableId"].ToString();
                                    string tabLable = tabReader["TableLabel"].ToString();
                                    MenuItem schemaOpenItem = new MenuItem();
                                    schemaOpenItem.Header = tabLable;
                                    schemaOpenItem.Click += new RoutedEventHandler((s, e) => { winConstruct(applicationTableId); });

                                    schemaOpen.Items.Add(schemaOpenItem);
                                };
                                ctrlTabDbCon.Close();
                            }
                            ctrlSchDbCon.Close();
                        };
                    }
                }

                ctrlAppDbCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Open connection:" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ctrlSchDbCon.Close();
                Window config = new Config();
                config.Show();
            };
            this.Show();
        }


        private void winConstruct(string applicationTableId)
        //Builds the window for the selected table 
        {
            SqlConnection appDbCon = new SqlConnection(Config.appDb.ToString());
            SqlConnection ctrlDbCon = new SqlConnection(Config.appDb.ToString());

            string controlName = string.Empty;
            string controlLabel = string.Empty;
            string controlRowSource = string.Empty;
            string controlFilter = string.Empty;
            string controlOrderBy = string.Empty;
            string controlType = string.Empty;
            string controlEnabled = string.Empty;

            Dictionary<string, string> controlValues = new Dictionary<string, string>();
            Int32 seletedFilter = 0;

            string appName = WindowTasks.winMetadataList(applicationTableId)[0];
            string tabKey = WindowTasks.winMetadataList(applicationTableId)[1];
            string tabName = WindowTasks.winMetadataList(applicationTableId)[2];
            string tabLabel = WindowTasks.winMetadataList(applicationTableId)[3];
            string schName = WindowTasks.winMetadataList(applicationTableId)[4];
            string schLable = WindowTasks.winMetadataList(applicationTableId)[5];


            //Create a new window - this is a window based on an underlying database table
            Window winNew = new Window();
            winNew.Style = (Style)FindResource("winStyle");
            winNew.Title = "Manage " + tabLabel + " (" + tabName + ")";
            winNew.Name = tabName;

            winNew.Resources.Add("tabId", applicationTableId);
            winNew.Resources.Add("winMode", "SELECT");
            winNew.Resources.Add("winFilter", "1 = 1");


            //Main layout Grid - 2 cols by 3 rows
            Grid mainGrid = new Grid();
            // NameScope.SetNameScope(mainGrid, new NameScope());

            //1st Column
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = GridLength.Auto;

            //2nd Column
            ColumnDefinition col2 = new ColumnDefinition();
            col1.Width = GridLength.Auto;

            //1st Row
            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            //2nd row
            RowDefinition row2 = new RowDefinition();
            row2.Height = GridLength.Auto;

            //3rd row
            RowDefinition row3 = new RowDefinition();
            row3.Height = GridLength.Auto;


            //Stack Panels
            //Window editing area
            StackPanel editStkPnl = new StackPanel();
            editStkPnl.Style = (Style)FindResource("winEditPanelStyle");
            editStkPnl.Name = "spEditArea";
            editStkPnl.Orientation = Orientation.Vertical;
            editStkPnl.Width = double.NaN;
            editStkPnl.VerticalAlignment = VerticalAlignment.Top;
            editStkPnl.Margin = new Thickness(10);
            NameScope.SetNameScope(editStkPnl, new NameScope());

            //Filter area
            StackPanel fltStkPnl = new StackPanel();
            NameScope.SetNameScope(fltStkPnl, new NameScope());

            //Button area
            StackPanel bttonStkPnl = new StackPanel();
            bttonStkPnl.Style = (Style)FindResource("winButtonStack");

            //Record selector area
            StackPanel RecordSelectorStkPnl = new StackPanel();
            RecordSelectorStkPnl.Style = (Style)FindResource("winPageSelectorStack");

            //Data tables
            //Window data grid - populated with a select query to underlying database table
            DataTable winDt = new DataTable();

            //Filter lists
            DataTable fltDt = new DataTable();

            //Data Grids
            //Window data list
            DataGrid winDg = new DataGrid();
            winDg.Style = (Style)FindResource("DataGridStyle");
            winDg.Tag = winNew.Tag;
            winDg.IsReadOnly = true;
            winDg.SelectionMode = DataGridSelectionMode.Single;

            //Other Controls
            //Combo for Filter Selector
            ComboBox winFlt = new ComboBox();
            winFlt.Name = "gridFilter";
            winFlt.Style = (Style)FindResource("winComboBoxStyle");

            //The buttons - Save, New, Delete, Exit
            Button btnSave = new Button();
            btnSave.Name = "btnSave";
            btnSave.Content = "Save";
            btnSave.Style = (Style)FindResource("winButtonStyle");

            Button btnNew = new Button();
            btnNew.Name = "btnNew";
            btnNew.Content = "New";
            btnNew.Style = (Style)FindResource("winButtonStyle");

            Button btnDelete = new Button();
            btnDelete.Name = "btnDel";
            btnDelete.Content = "Delete";
            btnDelete.Style = (Style)FindResource("winButtonStyle");

            Button btnClear = new Button();
            btnClear.Name = "btnClear";
            btnClear.Content = "Clear";
            btnClear.Style = (Style)FindResource("winButtonStyle");

            Button btnExit = new Button();
            btnExit.Name = "btnExit";
            btnExit.Content = "Exit";
            btnExit.Style = (Style)FindResource("winButtonStyle");

            //The record selector controls
            Button btnPrevPage = new Button();
            btnPrevPage.Name = "btnPrevPage";
            btnPrevPage.Content = "<";
            btnPrevPage.Style = (Style)FindResource("winTinyButtonStyle");

            Button btnNextPage = new Button();
            btnNextPage.Name = "btnNextPage";
            btnNextPage.Content = ">";
            btnNextPage.Style = (Style)FindResource("winTinyButtonStyle");

            TextBox tbSelectorText = new TextBox();
            tbSelectorText.Style = (Style)FindResource("winTinyTextBoxStyle");

            TextBox tbOffset = new TextBox();
            tbOffset.Visibility = Visibility.Collapsed;

            TextBox tbFetch = new TextBox();
            tbFetch.Visibility = Visibility.Collapsed;

            winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);

            //Populate Data tables
            //Window Filter - Gets the list of filters for the window based on the underlying database table
            SqlCommand getFltRows = new SqlCommand();
            getFltRows.CommandText =
                @"SELECT ApplicationFilterId AS valueMember,
                        FilterName AS displayMember
                FROM control.metadata.ApplicationFilter
                WHERE ApplicationTableId = @applicationTableId
                ORDER BY SortOrder";

            getFltRows.Parameters.AddWithValue("@applicationTableId", applicationTableId);
            getFltRows.CommandType = CommandType.Text;
            getFltRows.Connection = ctrlDbCon;

            ctrlDbCon.Open();
            {
                SqlDataAdapter fltAdapter = new SqlDataAdapter(getFltRows);
                DataTable fltDataTable = new DataTable();
                fltAdapter.Fill(fltDataTable);
                winFlt.ItemsSource = fltDataTable.DefaultView;
                winFlt.DisplayMemberPath = fltDataTable.Columns["displayMember"].ToString();
                winFlt.SelectedValuePath = fltDataTable.Columns["valueMember"].ToString();
            }
            ctrlDbCon.Close();

            //Set the Filter default value
            winFlt.SelectedIndex = 0;

            //Add controls to the window editing area Stack panel based on underlying database columns
            SqlCommand getColList = new SqlCommand();
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
                    WHERE ApplicationTableId = @applicationTableId
                    ORDER BY c.WindowLayoutOrder";

            getColList.Parameters.AddWithValue("@applicationTableId", applicationTableId);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = ctrlDbCon;


            ctrlDbCon.Open();
            try
            {
                {
                    SqlDataReader getColListReader = getColList.ExecuteReader();
                    while (getColListReader.Read())
                    {
                        controlName = getColListReader["ColumnName"].ToString();
                        controlLabel = getColListReader["ColumnLabel"].ToString();
                        controlRowSource = getColListReader["RowSource"].ToString();
                        controlFilter = getColListReader["Filter"].ToString();
                        controlOrderBy = getColListReader["OrderBy"].ToString();
                        controlType = getColListReader["WindowControlType"].ToString();
                        controlEnabled = getColListReader["WindowControlEnabled"].ToString();

                        controlValues.Add(controlName, null);

                        if (controlType != "ID")
                        {
                            Label lbl = new Label();
                            lbl.Content = controlLabel;
                            lbl.Style = (Style)FindResource("winLabelStyle");
                            editStkPnl.Children.Add(lbl);
                        }

                        switch (controlType)
                        {

                            case "ID":
                                TextBox rowKey = new TextBox();
                                rowKey.Name = controlName;
                                rowKey.Style = (Style)FindResource("winTextBoxStyle");
                                rowKey.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(rowKey);
                                editStkPnl.RegisterName(rowKey.Name, rowKey);
                                rowKey.TextChanged += new TextChangedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(rowKey, controlValues);
                                });
                                break;

                            case "TEXT":
                                TextBox tb = new TextBox();
                                tb.Name = controlName;
                                tb.Style = (Style)FindResource("winTextBoxStyle");
                                tb.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(tb);
                                editStkPnl.RegisterName(tb.Name, tb);
                                tb.TextChanged += new TextChangedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(tb, controlValues);
                                });
                                break;

                            case "TEXTBLOCK":
                            case "ROWSOURCE":
                            case "FILTER":
                            case "ORDERBY":
                                TextBox tbk = new TextBox();
                                tbk.Name = controlName;
                                tbk.Style = (Style)FindResource("winTextBlockStyle");
                                tbk.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(tbk);
                                editStkPnl.RegisterName(tbk.Name, tbk);
                                tbk.TextChanged += new TextChangedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(tbk, controlValues);
                                });
                                break;

                            case "NUM":
                                TextBox nb = new TextBox();
                                nb.Name = controlName;
                                nb.Style = (Style)FindResource("winNumBoxStyle");
                                nb.PreviewTextInput += numberValidationTextBox;
                                nb.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(nb);
                                editStkPnl.RegisterName(nb.Name, nb);
                                nb.TextChanged += new TextChangedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(nb, controlValues);
                                });
                                break;

                            case "CHK":
                                CheckBox chk = new CheckBox();
                                chk.Name = controlName;
                                //chk.Style = (Style)FindResource("winCheckBoxStyle");
                                chk.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(chk);
                                editStkPnl.RegisterName(chk.Name, chk);
                                chk.Checked += new RoutedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(chk, controlValues);
                                });
                                chk.Unchecked += new RoutedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(chk, controlValues);
                                });
                                chk.Indeterminate += new RoutedEventHandler((s, e) =>
                                {
                                    WindowDataOps.winGetControlValue(chk, controlValues);
                                });
                                break;

                            case "DATE":
                                DatePicker dtp = new DatePicker();
                                dtp.Name = controlName;
                                dtp.Style = (Style)FindResource("winDatePickerStyle");
                                dtp.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(dtp);
                                editStkPnl.RegisterName(dtp.Name, dtp);

                                break;

                            case "COMBO":
                                ComboBox cb = new ComboBox();
                                SqlCommand getComboRows = new SqlCommand();
                                string selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                                cb.Name = controlName;
                                cb.Style = (Style)FindResource("winComboBoxStyle");
                                cb.IsEnabled = Convert.ToBoolean(controlEnabled);
                                cb.SelectedValuePath = "valueMember";
                                cb.DisplayMemberPath = "displayMember";
                                cb.DropDownClosed += new EventHandler((s, e) =>
                                {
                                    if (cb.SelectedValue != null)
                                    {
                                        WindowDataOps.winGetControlValue(cb, controlValues);
                                        if (winNew.Resources["winMode"].ToString() != "EDIT")
                                        {
                                            dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                                        }

                                    }
                                });

                                cb.DropDownOpened += new EventHandler((s, e) =>
                                {
                                    DataTable comboDataTable = new DataTable();
                                    comboDataTable = WindowDataOps.winPopulateCombo(cb, applicationTableId, cb.Name, ctrlDbCon, appDbCon, editStkPnl, controlValues, winDg);
                                    cb.ItemsSource = comboDataTable.DefaultView;
                                    cb.DisplayMemberPath = comboDataTable.Columns["displayMember"].ToString();
                                    cb.SelectedValuePath = comboDataTable.Columns["valueMember"].ToString();
                                });
                                //Populate Combo

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
                                    DataTable comboDataTable = new DataTable();

                                    comboAdapter.Fill(comboDataTable);
                                    cb.ItemsSource = comboDataTable.DefaultView;
                                    cb.DisplayMemberPath = comboDataTable.Columns["displayMember"].ToString();
                                    cb.SelectedValuePath = comboDataTable.Columns["valueMember"].ToString();
                                    editStkPnl.Children.Add(cb);
                                    editStkPnl.RegisterName(cb.Name, cb);
                                }
                                appDbCon.Close();

                                break;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                WindowTasks.DisplayError(ex, "Cannot Build Form Control:" + ex.Message, controlRowSource);
                ctrlDbCon.Close();
            }
            ctrlDbCon.Close();

            //Event Handler's

            //Data Grid
            winDg.SelectionChanged += new SelectionChangedEventHandler((s, e) =>
            {
                if (winDg.SelectedItem == null) return;

                WindowTasks.winSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                dataGridClicked(applicationTableId, winDg, editStkPnl, controlValues);
            });

            //Filter Selector
            winFlt.DropDownClosed += new EventHandler((s, e) =>
            {
                ComboBox clicked = (ComboBox)s;
                seletedFilter = (Int32)clicked.SelectedValue;
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);
                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
            }
            );

            //buttons
            btnSave.Click += new RoutedEventHandler((s, e) =>
            {
                switch (winNew.Resources["winMode"].ToString())
                {
                    case "NEW":
                        dbCreateRecord(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                        winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                        WindowTasks.winSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                        break;
                    case "EDIT":
                        dbUpdateRecord(winNew, applicationTableId, winDg, editStkPnl, fltStkPnl, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                        WindowTasks.winSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                        break;
                }

            });
            btnNew.Click += new RoutedEventHandler((s, e) =>
            {
                WindowTasks.winSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
            });
            btnDelete.Click += new RoutedEventHandler((s, e) =>
            {
                dbDeleteRecord(winNew, applicationTableId, fltStkPnl, winDg, editStkPnl, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
            });
            btnExit.Click += new RoutedEventHandler(WindowTasks.winClose);
            btnClear.Click += new RoutedEventHandler((s, e) =>
            {
                seletedFilter = 0;
                winFlt.SelectedIndex = seletedFilter;
                winClearDataFields(winNew, editStkPnl, fltStkPnl, false);
                winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                WindowDataOps.winClearControlDictionaryValues(controlValues);
                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            });
            tbOffset.TextChanged += new TextChangedEventHandler((s, e) =>
            {
                dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
            });

            btnNextPage.Click += new RoutedEventHandler((s, e) =>
              {
                  tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) + Convert.ToInt32(tbFetch.Text));
                  winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
              });

            btnPrevPage.Click += new RoutedEventHandler((s, e) =>
            {
                if (Convert.ToInt32(tbOffset.Text) >= Convert.ToInt32(tbFetch.Text))
                {
                    tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) - Convert.ToInt32(tbFetch.Text));
                    winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                }
            });

            //Add Rows and Columns to the Grid
            mainGrid.ColumnDefinitions.Add(col1);
            mainGrid.ColumnDefinitions.Add(col2);

            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);
            mainGrid.RowDefinitions.Add(row3);

            //Add the datagrid to the window
            Grid.SetColumn(winDg, 1);
            Grid.SetRow(winDg, 1);
            Grid.SetRowSpan(winDg, 1);

            mainGrid.Children.Add(winDg);

            //Add combo/text to stack panel
            fltStkPnl.Children.Add(winFlt);

            //Add Filter selector to Window
            Grid.SetColumn(fltStkPnl, 1);
            Grid.SetRow(fltStkPnl, 0);
            mainGrid.Children.Add(fltStkPnl);

            //Add editing controls Stack Panel
            Grid.SetColumn(editStkPnl, 0);
            Grid.SetRow(editStkPnl, 1);

            mainGrid.Children.Add(editStkPnl);

            //Add Editing buttons
            Grid.SetColumn(bttonStkPnl, 0);
            Grid.SetColumnSpan(bttonStkPnl, 1);
            Grid.SetRow(bttonStkPnl, 2);

            bttonStkPnl.Children.Add(btnSave);
            bttonStkPnl.Children.Add(btnNew);
            bttonStkPnl.Children.Add(btnDelete);
            bttonStkPnl.Children.Add(btnClear);
            bttonStkPnl.Children.Add(btnExit);

            mainGrid.Children.Add(bttonStkPnl);

            //Add Record selector
            Grid.SetColumn(RecordSelectorStkPnl, 1);
            Grid.SetRow(RecordSelectorStkPnl, 2);

            RecordSelectorStkPnl.Children.Add(tbOffset);
            RecordSelectorStkPnl.Children.Add(btnPrevPage);
            RecordSelectorStkPnl.Children.Add(tbSelectorText);
            RecordSelectorStkPnl.Children.Add(btnNextPage);
            RecordSelectorStkPnl.Children.Add(tbFetch);

            mainGrid.Children.Add(RecordSelectorStkPnl);

            winNew.Content = mainGrid;
            winNew.SizeToContent = SizeToContent.WidthAndHeight;
            winNew.Show();


            dbGetDataGridRows(winNew, applicationTableId, editStkPnl, fltStkPnl, winDg, 0, controlValues, tbOffset, tbFetch, tbSelectorText);
            WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
        }
    }
}