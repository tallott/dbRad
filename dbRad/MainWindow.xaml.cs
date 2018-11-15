﻿using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using dbRad.Classes;
using Npgsql;

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
                Config.applicationUser = ApplicationFiletasks.ReadFromXmlFile<ApplicationUser>(Config.userFilePath);
            }
            if (File.Exists(Config.appDbFilePath))
            {
                Config.appDb = ApplicationFiletasks.ReadFromXmlFile<ApplicationConnections>(Config.appDbFilePath);
            }

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


        private void showConfig(object sender, EventArgs e)
        //Open config window
        {
            Window winConfig = new Config();
            winConfig.Show();
        }

        private void mainWinBuild()

        //Builds the main window for the appDbName
        {
            NpgsqlConnection ctrlAppDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("control"));
            NpgsqlConnection ctrlSchDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("control"));
            NpgsqlConnection ctrlTabDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("control"));

            //Get configured Application database
            string appDbName;
            string appHost = Config.appDb.HostName;
            string userName = Config.applicationUser.UserName;
            string userPassword = Config.applicationUser.UserPassword;
            string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Title = "Connection - " + userName + "@" + appHost + " (Software Version:" + version + ")";

            //Build Main Menu

            //A File Menu
            MenuItem fileMenu = new MenuItem();
            fileMenu.Header = "File";
            menu.Items.Add(fileMenu);

            //Item 1 Exit
            MenuItem fileExitItem = new MenuItem();
            fileExitItem.Header = "Exit";
            fileExitItem.Click += new RoutedEventHandler(ApplicationUtils.appShutdown);
            fileMenu.Items.Add(fileExitItem);

            //A config Menu
            MenuItem configMenu = new MenuItem();
            configMenu.Header = "Config";
            menu.Items.Add(configMenu);

            //Item 1 Database
            MenuItem databaseConfigItem = new MenuItem();
            databaseConfigItem.Header = "Settings";
            databaseConfigItem.Click += new RoutedEventHandler(showConfig);
            configMenu.Items.Add(databaseConfigItem);

            //An Open Menu
            MenuItem openMenu = new MenuItem();
            openMenu.Header = "Open";
            menu.Items.Add(openMenu);

            //Open Menu Items are Dynamicly populated with application/schema/table names in control tables;
            try
            {
                NpgsqlCommand getAppList = new NpgsqlCommand();
                getAppList.CommandText = ControlDatabaseSql.ApplicationList();
                   

                getAppList.CommandType = CommandType.Text;
                getAppList.Parameters.AddWithValue("@UserName", userName);
                getAppList.Parameters.AddWithValue("@UserPassword", userPassword);

                getAppList.Connection = ctrlAppDbCon;
                ctrlAppDbCon.Open();

                NpgsqlDataReader appReader = getAppList.ExecuteReader();
                while (appReader.Read())
                {
                    string applicationId = appReader["application_id"].ToString();
                    appDbName = appReader["application_name"].ToString();

                    MenuItem AppOpen = new MenuItem();
                    AppOpen.Header = appDbName;
                    openMenu.Items.Add(AppOpen);
                    {
                        NpgsqlCommand getSchList = new NpgsqlCommand();
                        getSchList.CommandText = ControlDatabaseSql.SchemaList();

                        ctrlSchDbCon.Open();
                        getSchList.CommandType = CommandType.Text;
                        getSchList.Parameters.AddWithValue("@appDbName", appDbName);
                        getSchList.Parameters.AddWithValue("@UserName", userName);
                        getSchList.Parameters.AddWithValue("@UserPassword", userPassword);
                        getSchList.Connection = ctrlSchDbCon;
                        {
                            NpgsqlDataReader schReader = getSchList.ExecuteReader();

                            while (schReader.Read())
                            {
                                Int32 schemaId = Convert.ToInt32(schReader["application_schema_Id"]);
                                string schemaName = schReader["schema_label"].ToString();
                                MenuItem schemaOpen = new MenuItem();
                                schemaOpen.Header = schemaName;
                                AppOpen.Items.Add(schemaOpen);
                                NpgsqlCommand getTabList = new NpgsqlCommand();

                                getTabList.CommandText = ControlDatabaseSql.TableList();
                       
                                getTabList.CommandType = CommandType.Text;

                                getTabList.Connection = ctrlTabDbCon;
                                ctrlTabDbCon.Open();
                                getTabList.Parameters.AddWithValue("@appDbName", appDbName);
                                getTabList.Parameters.AddWithValue("@UserName", userName);
                                getTabList.Parameters.AddWithValue("@UserPassword", userPassword);
                                getTabList.Parameters.AddWithValue("@ApplicationSchemaId", schemaId);
                                NpgsqlDataReader tabReader = getTabList.ExecuteReader();
                                while (tabReader.Read())
                                {
                                    Int32 applicationTableId = Convert.ToInt32(tabReader["application_table_id"]);
                                    string tableLabel = tabReader["table_label"].ToString();
                                    MenuItem schemaOpenItem = new MenuItem();
                                    schemaOpenItem.Header = tableLabel;
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
            Show();
        }

        private void winConstruct(Int32 applicationTableId)
        //Builds the window for the selected table 
        {


            string controlName = string.Empty;
            string controlLabel = string.Empty;
            string controlRowSource = string.Empty;
            string controlFilter = string.Empty;
            string controlOrderBy = string.Empty;
            string controlType = string.Empty;
            string controlEnabled = string.Empty;
            string controlDefaultValue = string.Empty;

            Dictionary<string, string> controlValues = new Dictionary<string, string>();
            Dictionary<string, string> controlValueDefaults = new Dictionary<string, string>();

            Int32 selectedDataGridRowIdVal = 0;
            Int32 selectedFilter = 0;


            string displayMember = String.Empty;

            WindowMetaList windowMetaList = WindowTasks.winMetadataList(applicationTableId);

            NpgsqlConnection appDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString(windowMetaList.ApplicationName));
            NpgsqlConnection ctrlDbCon = new NpgsqlConnection(ApplicationEnviroment.ConnectionString("control"));

            //Create a new window - this is a window based on an underlying database table
            Window winNew = new Window();
            winNew.Style = (Style)FindResource("winStyle");
            winNew.Title = "Manage " + windowMetaList.TableLabel + " (" + windowMetaList.TableName + ")";
            winNew.Name = windowMetaList.TableName;
            winNew.Activated += new EventHandler((s, e) =>
           {
               windowMetaList = WindowTasks.winMetadataList(applicationTableId);
           });

            //Main layout Grid - 2 cols by 3 rows
            Grid mainGrid = new Grid();

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

            //Data Grids
            //Window data list
            DataGrid winDg = new DataGrid();
            winDg.Style = (Style)FindResource("DataGridStyle");
            winDg.Tag = winNew.Tag;
            winDg.IsReadOnly = true;
            winDg.SelectionMode = DataGridSelectionMode.Single;

            //Stack Panels
            //Window editing area
            StackPanel editStkPnl = new StackPanel();
            //editStkPnl.DataContext = winDt;
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

            WindowTasks.winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);

            //Populate Data tables
            //Window Filter - Gets the list of filters for the window based on the underlying database table
            NpgsqlCommand getFltRows = new NpgsqlCommand();
            getFltRows.CommandText = ControlDatabaseSql.TableFilterList();
   
            getFltRows.Parameters.AddWithValue("@applicationTableId", windowMetaList.TableId);
            getFltRows.CommandType = CommandType.Text;
            getFltRows.Connection = ctrlDbCon;

            ctrlDbCon.Open();
            {
                NpgsqlDataAdapter fltAdapter = new NpgsqlDataAdapter(getFltRows);
                DataTable fltDataTable = new DataTable();
                fltAdapter.Fill(fltDataTable);
                winFlt.ItemsSource = fltDataTable.DefaultView;
                winFlt.DisplayMemberPath = fltDataTable.Columns["display_member"].ToString();
                winFlt.SelectedValuePath = fltDataTable.Columns["value_member"].ToString();
            }
            ctrlDbCon.Close();

            //Set the Filter default value
            winFlt.SelectedIndex = 0;

            //Add controls to the window editing area Stack panel based on underlying database columns
            NpgsqlCommand getColList = new NpgsqlCommand();
            getColList.CommandText = ControlDatabaseSql.ColumnMetadata();
                 
            getColList.Parameters.AddWithValue("@applicationTableId", windowMetaList.TableId);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = ctrlDbCon;


            ctrlDbCon.Open();
            try
            {
                {
                    NpgsqlDataReader getColListReader = getColList.ExecuteReader();
                    while (getColListReader.Read())
                    {
                        controlName = getColListReader["column_name"].ToString().ToLower();
                        controlLabel = getColListReader["column_label"].ToString();
                        controlRowSource = getColListReader["row_source"].ToString();
                        controlFilter = getColListReader["filter"].ToString();
                        controlOrderBy = getColListReader["order_by"].ToString();
                        controlType = getColListReader["window_control_type"].ToString();
                        controlEnabled = getColListReader["window_control_enabled"].ToString();
                        controlDefaultValue = getColListReader["column_default_value"].ToString();

                        controlValues.Add(controlName, null);
                        controlValueDefaults.Add(controlName, controlDefaultValue);

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
                                rowKey.Tag = controlType;
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
                                tb.Tag = controlType;
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
                                tbk.Tag = controlType;
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
                                nb.Tag = controlType;
                                nb.PreviewTextInput += ApplicationUtils.numberValidationTextBox;
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
                                chk.Tag = controlType;
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
                                dtp.Tag = controlType;
                                dtp.IsEnabled = Convert.ToBoolean(controlEnabled);
                                editStkPnl.Children.Add(dtp);
                                editStkPnl.RegisterName(dtp.Name, dtp);

                                break;

                            case "COMBO":
                                ComboBox cb = new ComboBox();
                                NpgsqlCommand getComboRows = new NpgsqlCommand();
                                Int32 selectedRowIdVal = WindowTasks.dataGridGetId(winDg);

                                cb.Name = controlName;
                                cb.Style = (Style)FindResource("winComboBoxStyle");
                                cb.Tag = controlType;
                                cb.IsEnabled = Convert.ToBoolean(controlEnabled);
                                cb.SelectedValuePath = "value_member";
                                cb.DisplayMemberPath = "display_member";
                                cb.DropDownClosed += new EventHandler((s, e) =>
                                {
                                    if (cb.SelectedValue != null)
                                    {
                                        WindowDataOps.winGetControlValue(cb, controlValues);
                                        if (winNew.Resources["winMode"].ToString() != "EDIT")
                                        {
                                            DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                                        }
                                    }
                                    else
                                    {
                                        cb.Text = displayMember;
                                    }
                                });

                                cb.DropDownOpened += new EventHandler((s, e) =>
                                {
                                    displayMember = cb.Text;
                                    DataTable comboDataTable = new DataTable();
                                    comboDataTable = WindowDataOps.winPopulateCombo(cb, windowMetaList, cb.Name, ctrlDbCon, appDbCon, editStkPnl, controlValues, winDg);
                                    cb.ItemsSource = comboDataTable.DefaultView;
                                    cb.DisplayMemberPath = comboDataTable.Columns["display_member"].ToString();
                                    cb.SelectedValuePath = comboDataTable.Columns["value_member"].ToString();
                                });
                                //Populate Combo

                                if (controlOrderBy == string.Empty)
                                    controlOrderBy = "\nORDER BY 1";
                                else
                                    controlOrderBy = "\nORDER BY " + controlOrderBy;

                                controlRowSource += controlOrderBy;
                                controlRowSource = WindowDataOps.SubstituteWindowParameters(controlRowSource, controlValues);

                                getComboRows.CommandText = controlRowSource;
                                getComboRows.CommandType = CommandType.Text;
                                getComboRows.Connection = appDbCon;

                                appDbCon.Open();

                                {
                                    NpgsqlDataAdapter comboAdapter = new NpgsqlDataAdapter(getComboRows);
                                    DataTable comboDataTable = new DataTable();

                                    comboAdapter.Fill(comboDataTable);
                                    cb.ItemsSource = comboDataTable.DefaultView;
                                    cb.DisplayMemberPath = comboDataTable.Columns["display_member"].ToString();
                                    cb.SelectedValuePath = comboDataTable.Columns["value_member"].ToString();
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
                WindowDataOps.winDataGridClicked(windowMetaList, winDg, 0, editStkPnl, controlValues);
            });

            //Filter Selector
            winFlt.DropDownClosed += new EventHandler((s, e) =>
            {
                ComboBox clicked = (ComboBox)s;
                selectedFilter = (Int32)clicked.SelectedValue;
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                WindowTasks.winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);
                DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                WindowTasks.winSetControlDefaultValues(editStkPnl, controlValueDefaults);
            }
            );

            //buttons
            btnSave.Click += new RoutedEventHandler((s, e) =>
            {
                switch (winNew.Resources["winMode"].ToString())
                {
                    case "NEW":
                        if (DatabaseDataOps.dbCreateRecord(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText) == true)
                        {
                            WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                            WindowTasks.winSetControlDefaultValues(editStkPnl, controlValueDefaults);
                            WindowTasks.winSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                            DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                        }
                        break;
                    case "EDIT":
                        selectedDataGridRowIdVal = WindowTasks.dataGridGetId(winDg);

                        if (DatabaseDataOps.dbUpdateRecord(windowMetaList, winDg, editStkPnl) == true)
                        {
                            WindowTasks.winSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                            DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                            WindowTasks.winDataGridSelectRow(selectedDataGridRowIdVal, winDg);
                            WindowDataOps.winDataGridClicked(windowMetaList, winDg, selectedDataGridRowIdVal, editStkPnl, controlValues);
                        }
                        break;
                }

            });
            btnNew.Click += new RoutedEventHandler((s, e) =>
            {
                WindowTasks.winSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                WindowTasks.winSetControlDefaultValues(editStkPnl, controlValueDefaults);
            });
            btnDelete.Click += new RoutedEventHandler((s, e) =>
            {
                DatabaseDataOps.dbDeleteRecord(windowMetaList, winDg);

                DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                WindowTasks.winSetControlDefaultValues(editStkPnl, controlValueDefaults);
            });
            btnExit.Click += new RoutedEventHandler(WindowTasks.winClose);
            btnClear.Click += new RoutedEventHandler((s, e) =>
            {
                selectedFilter = 0;
                winFlt.SelectedIndex = selectedFilter;
                WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, false);
                WindowTasks.winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                WindowDataOps.winClearControlDictionaryValues(controlValues);
                DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            });
            tbOffset.TextChanged += new TextChangedEventHandler((s, e) =>
            {
                DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
            });

            btnNextPage.Click += new RoutedEventHandler((s, e) =>
              {
                  tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) + Convert.ToInt32(tbFetch.Text));
                  WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
              });

            btnPrevPage.Click += new RoutedEventHandler((s, e) =>
            {
                if (Convert.ToInt32(tbOffset.Text) >= Convert.ToInt32(tbFetch.Text))
                {
                    tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) - Convert.ToInt32(tbFetch.Text));
                    WindowTasks.winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
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


            DatabaseDataOps.dbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, winDg, 0, controlValues, tbOffset, tbFetch, tbSelectorText);
            WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
        }
    }
}