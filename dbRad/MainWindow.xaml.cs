using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using dbRad.Classes;
using Npgsql;
using System.Linq;

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
            AppStartup();
        }

        private void AppStartup()
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
                MainWinBuild(false);
            }

        }


        private void ShowConfig(object sender, EventArgs e)
        //Open config window
        {
            this.Hide();
            Window winConfig = new Config();
            winConfig.ShowDialog();
            MainWinBuild(true);
            this.Show();
        }

        void FileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
        }

        private void MainWinBuild(Boolean IsRefresh)

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

            if (IsRefresh)
            {
                for (int i = this.menuGrid.Children.Count - 1; i >= 0; --i)
                {
                    var childTypeName = menuGrid.Children[i].GetType().Name;
                    if (childTypeName == "Menu")
                    {
                        menuGrid.Children.RemoveAt(i);
                    }
                }

            }

            Menu menu = new Menu
            {
                Name = "menu",
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 27,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 1367
            };
            menuGrid.Children.Add(menu);


            //A File Menu
            MenuItem fileMenu = new MenuItem
            {
                Header = "File"
            };
            menu.Items.Add(fileMenu);

            //Item 1 Exit
            MenuItem fileExitItem = new MenuItem
            {
                Header = "Exit"
            };
            fileExitItem.Click += new RoutedEventHandler(FileExitMenuItem_Click);
            fileMenu.Items.Add(fileExitItem);

            //A config Menu
            MenuItem configMenu = new MenuItem
            {
                Header = "Config"
            };
            menu.Items.Add(configMenu);

            //Item 1 Database
            MenuItem databaseConfigItem = new MenuItem
            {
                Header = "Settings"
            };
            databaseConfigItem.Click += new RoutedEventHandler(ShowConfig);
            configMenu.Items.Add(databaseConfigItem);

            //An Open Menu
            MenuItem openMenu = new MenuItem
            {
                Header = "Open"
            };
            menu.Items.Add(openMenu);

            //Open Menu Items are Dynamicly populated with application/schema/table names in control tables;
            try
            {
                NpgsqlCommand getAppList = new NpgsqlCommand
                {
                    CommandText = ControlDatabaseSql.ApplicationList(),
                    CommandType = CommandType.Text,
                    Connection = ctrlAppDbCon
                };

                getAppList.Parameters.AddWithValue("@UserName", userName);
                getAppList.Parameters.AddWithValue("@UserPassword", userPassword);


                ctrlAppDbCon.Open();

                NpgsqlDataReader appReader = getAppList.ExecuteReader();
                while (appReader.Read())
                {
                    string applicationId = appReader["application_id"].ToString();
                    appDbName = appReader["application_name"].ToString();

                    MenuItem AppOpen = new MenuItem
                    {
                        Header = appDbName
                    };
                    openMenu.Items.Add(AppOpen);
                    {
                        NpgsqlCommand getSchList = new NpgsqlCommand
                        {
                            CommandText = ControlDatabaseSql.SchemaList()
                        };

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
                                MenuItem schemaOpen = new MenuItem
                                {
                                    Header = schemaName
                                };
                                AppOpen.Items.Add(schemaOpen);
                                NpgsqlCommand getTabList = new NpgsqlCommand
                                {
                                    CommandText = ControlDatabaseSql.TableList(),
                                    CommandType = CommandType.Text,
                                    Connection = ctrlTabDbCon
                                };
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
                                    MenuItem schemaOpenItem = new MenuItem
                                    {
                                        Header = tableLabel
                                    };
                                    schemaOpenItem.Click += new RoutedEventHandler((s, e) => { WinConstruct(applicationTableId); });

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
                this.Hide();
                Window config = new Config();
                config.ShowDialog();
                MainWinBuild(true);
            };
            Show();
        }

        private void WinConstruct(Int32 applicationTableId)
        //Builds the window for applicationTableId
        {
            string controlStyle = null;

            //These dictionaries are used to pass control values between classes for each instance of a window
            Dictionary<string, string> controlValues = new Dictionary<string, string>();
            //Dictionary<string, string> controlValueDefaults = new Dictionary<string, string>();
            //Dictionary<string, string> controlValueRequired = new Dictionary<string, string>();

            Int32 selectedDataGridRowIdVal = 0;
            Int32 selectedFilter = 0;

            string displayMember = String.Empty;

            //Set the metadata values specific to the applicationTableId for the instance of the window
            WindowMetaList windowMetaList = WindowTasks.WinMetadataList(applicationTableId);

            //Create a new window - we will add various UI elements to the window based on the applicationTableId
            Window winNew = WindowBuildUtils.CreateWindow(windowMetaList, applicationTableId);

            //Main layout Grid - 2 cols by 3 rows
            Grid mainGrid = WindowBuildUtils.CreateMainGrid();

            //Data Grids
            //Create a Grid containing a DataGrid to display rows from the underlying applicatioin table
            Grid dataGrid = new Grid();
            DataGrid windowDataGrid = WindowBuildUtils.CreateDataGridInGrid("DataGridStyle", "DataGridGridStyle", "ControlBorderStyle", dataGrid);

            //A Grid containing a StackPanel for the Editing controls
            Grid editGrid = new Grid();
            StackPanel editStkPnl = WindowBuildUtils.CreateStackPanelInGrid("winEditPanelStyle", "ControlGridStyle", "ControlBorderStyle", editGrid);
            NameScope.SetNameScope(editStkPnl, new NameScope());

            //A Grid containg a StackPanel for the Filter selector
            Grid fltGrid = new Grid();
            StackPanel fltStkPnl = WindowBuildUtils.CreateStackPanelInGrid("winFilterStack", "ControlGridStyle", "ControlBorderStyle", fltGrid);

            //A Grid containing a StackPane for the Buttons
            Grid buttonGrid = new Grid();
            StackPanel buttonStkPnl = WindowBuildUtils.CreateStackPanelInGrid("winButtonStack", "ControlGridStyle", "ControlBorderStyle", buttonGrid);

            //A Grid containg a StackPanel for the Record selectors
            Grid recordSelectorGrid = new Grid();
            StackPanel recordSelectorStkPnl = WindowBuildUtils.CreateStackPanelInGrid("winPageSelectorStack", "ControlGridStyle", "ControlBorderStyle", recordSelectorGrid);

            //A Grid containing a StackPanel for the Message area
            Grid messageGrid = new Grid();
            StackPanel messageStkPnl = WindowBuildUtils.CreateStackPanelInGrid("winMessageStack", "ControlGridStyle", "ControlBorderStyle", messageGrid);

            //Other Controls
            //Label & Combo for Filter Selector
            Label labelFlt = WindowBuildUtils.CreateLabel("Select Filter", "winLabelStyle");
            ComboBox winFlt = WindowBuildUtils.CreateFilterCombo(windowMetaList);

            //The buttons
            Button btnSave = WindowBuildUtils.CreateButton("btnSave", "Save", "winButtonStyle");
            Button btnNew = WindowBuildUtils.CreateButton("btnNew", "New", "winButtonStyle");
            Button btnDelete = WindowBuildUtils.CreateButton("btnDel", "Delete", "winButtonStyle");
            Button btnClear = WindowBuildUtils.CreateButton("btnClear", "Clear", "winButtonStyle");
            Button btnExit = WindowBuildUtils.CreateButton("btnExit", "Exit", "winButtonStyle");

            //The record selector controls
            Button btnPrevPage = WindowBuildUtils.CreateButton("btnPrevPage", "<", "winTinyButtonStyle");
            Button btnNextPage = WindowBuildUtils.CreateButton("btnNextPage", ">", "winTinyButtonStyle");
            TextBox tbSelectorText = WindowBuildUtils.CreateTextBox("tbSelectorText", string.Empty, "winTinyTextBoxStyle", Visibility.Visible);
            TextBox tbOffset = WindowBuildUtils.CreateTextBox("tbOffset", "0", "winTinyTextBoxStyle", Visibility.Collapsed);
            TextBox tbFetch = WindowBuildUtils.CreateTextBox("tbFetch", windowMetaList.PageRowCount, "winTinyTextBoxStyle", Visibility.Collapsed);

            //The Message control
            TextBox tbWinMode = WindowBuildUtils.CreateTextBox("tbWinMode", string.Empty, "winMessageTextBoxStyle", Visibility.Collapsed);

            //Create and Add controls to the window editing area Stack panel based on underlying database columns
            foreach(var columns in windowMetaList.Columns)
            {
                controlValues.Add(columns.ColumnName, null);
                string toolTip = columns.ColumnLabel + "\r\n\n" + "Required = " + columns.ColumnRequiredValue + "\r\n\n" + columns.ColumnDescription;

                if (columns.ColumnType != "ID")
                {
                    switch (columns.ColumnRequiredValue)
                    {
                        case "True":
                            controlStyle = "winLabelRequiredStyle";
                            break;
                        case "False":
                            controlStyle = "winLabelStyle";
                            break;
                    }

                    Label lbl = WindowBuildUtils.CreateLabel(columns.ColumnLabel, controlStyle);

                    editStkPnl.Children.Add(lbl);
                }

                switch (columns.ColumnType)
                {

                    case "ID":
                        TextBox rowKey = WindowBuildUtils.CreateTextBox(columns.ColumnName, "winTextBoxStyle", columns.ColumnEnabled, columns.ColumnType, toolTip);
                        rowKey.TextChanged += new TextChangedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(rowKey, controlValues);
                        });
                        editStkPnl.Children.Add(rowKey);
                        editStkPnl.RegisterName(rowKey.Name, rowKey);
                        break;

                    case "TEXT":
                        switch (columns.ColumnRequiredValue)
                        {
                            case "True":
                                controlStyle = "winTextBoxRequiredStyle";
                                break;
                            case "False":
                                controlStyle = "winTextBoxStyle";
                                break;
                        }
                        TextBox tb = WindowBuildUtils.CreateTextBox(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        tb.TextChanged += new TextChangedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(tb, controlValues);
                        });
                        editStkPnl.Children.Add(tb);
                        editStkPnl.RegisterName(tb.Name, tb);
                        break;

                    case "TEXTBLOCK":
                    case "ROWSOURCE":
                    case "FILTER":
                    case "ORDERBY":
                        switch (columns.ColumnRequiredValue)
                        {
                            case "True":
                                controlStyle = "winTextBlockRequiredStyle";
                                break;
                            case "False":
                                controlStyle = "winTextBlockStyle";
                                break;
                        }
                        TextBox tbk = WindowBuildUtils.CreateTextBox(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        tbk.TextChanged += new TextChangedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(tbk, controlValues);
                        });
                        editStkPnl.Children.Add(tbk);
                        editStkPnl.RegisterName(tbk.Name, tbk);
                        break;

                    case "NUM":
                        controlStyle = "winNumBoxStyle";
                        TextBox nb = WindowBuildUtils.CreateTextBox(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        nb.PreviewTextInput += ApplicationUtils.NumberValidationTextBox;
                        nb.TextChanged += new TextChangedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(nb, controlValues);
                        });

                        editStkPnl.Children.Add(nb);
                        editStkPnl.RegisterName(nb.Name, nb);

                        break;

                    case "CHK":
                        controlStyle = "winCheckBoxStyle";
                        CheckBox chk = WindowBuildUtils.CreateCheckBox(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        chk.Checked += new RoutedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(chk, controlValues);
                        });
                        chk.Unchecked += new RoutedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(chk, controlValues);
                        });
                        chk.Indeterminate += new RoutedEventHandler((s, e) =>
                        {
                            WindowDataOps.WinGetControlValue(chk, controlValues);
                        });
                        editStkPnl.Children.Add(chk);
                        editStkPnl.RegisterName(chk.Name, chk);
                        break;

                    case "DATE":
                        controlStyle = "winDatePickerStyle";
                        DatePicker dtp = WindowBuildUtils.CreateDatePicker(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        editStkPnl.Children.Add(dtp);
                        editStkPnl.RegisterName(dtp.Name, dtp);

                        break;

                    case "COMBO":
                        controlStyle = "winComboBoxStyle";
                        ComboBox cb = WindowBuildUtils.CreateComboBox(columns.ColumnName, controlStyle, columns.ColumnEnabled, columns.ColumnType, toolTip);

                        cb.DropDownClosed += new EventHandler((s, e) =>
                        {
                            if (cb.SelectedValue != null)
                            {
                                WindowDataOps.WinGetControlValue(cb, controlValues);
                                if (windowMetaList.WinMode != "EDIT")
                                {
                                    DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);
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
                            comboDataTable = WindowDataOps.WinPopulateCombo(cb, windowMetaList, cb.Name, controlValues);
                            cb.ItemsSource = comboDataTable.DefaultView;
                            cb.DisplayMemberPath = comboDataTable.Columns["display_member"].ToString();
                            cb.SelectedValuePath = comboDataTable.Columns["value_member"].ToString();
                        });
                        //Populate Combo

                        if (columns.ColumnOrderBy == string.Empty)
                            columns.ColumnOrderBy = "\nORDER BY 1";
                        else
                            columns.ColumnOrderBy = "\nORDER BY " + columns.ColumnOrderBy;

                        columns.ColumnRowSource += columns.ColumnOrderBy;
                        columns.ColumnRowSource = WindowDataOps.SubstituteWindowParameters(columns.ColumnRowSource, controlValues);

                        NpgsqlCommand getComboRows = new NpgsqlCommand();
                        Int32 selectedRowIdVal = WindowTasks.DataGridGetId(windowDataGrid);
                        getComboRows.CommandText = columns.ColumnRowSource;
                        getComboRows.CommandType = CommandType.Text;
                        getComboRows.Connection = windowMetaList.ApplicationDb;

                        windowMetaList.ApplicationDb.Open();

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
                        windowMetaList.ApplicationDb.Close();

                        break;
                }
               
            }


            //Event Handler's

            //Data Grid
            windowDataGrid.SelectionChanged += new SelectionChangedEventHandler((s, e) =>
            {
                if (windowDataGrid.SelectedItem == null) return;
                windowMetaList.GridSelectedIndex = windowDataGrid.SelectedIndex;
                WindowTasks.WinSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                WindowDataOps.WinDataGridClicked(windowMetaList, windowDataGrid, 0, editStkPnl, controlValues);
            });

            //Filter Selector
            winFlt.DropDownClosed += new EventHandler((s, e) =>
            {
                ComboBox clicked = (ComboBox)s;
                selectedFilter = (Int32)clicked.SelectedValue;
                WindowTasks.WinSetMode("CLEAR", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                WindowTasks.WinResetRecordSelector(tbSelectorText, tbOffset, tbFetch, windowMetaList);
                DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);
                WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
                WindowTasks.WinSetControlDefaultValues(editStkPnl, windowMetaList);
            }
            );

            //buttons
            btnSave.Click += new RoutedEventHandler((s, e) =>
            {
                switch (windowMetaList.WinMode)
                {
                    case "NEW":
                        if (DatabaseDataOps.DbCreateRecord(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbFetch, tbSelectorText) == true)
                        {
                            WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
                            WindowTasks.WinSetControlDefaultValues(editStkPnl, windowMetaList);
                            WindowTasks.WinSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                            DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);

                        }
                        break;
                    case "EDIT":
                        selectedDataGridRowIdVal = WindowTasks.DataGridGetId(windowDataGrid);

                        if (DatabaseDataOps.DbUpdateRecord(windowMetaList, windowDataGrid, editStkPnl) == true)
                        {
                            WindowTasks.WinSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                            DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);
                            WindowTasks.WinDataGridSelectRow(selectedDataGridRowIdVal, windowDataGrid, windowMetaList);
                            WindowDataOps.WinDataGridClicked(windowMetaList, windowDataGrid, selectedDataGridRowIdVal, editStkPnl, controlValues);
                        }
                        break;
                }

            });
            btnNew.Click += new RoutedEventHandler((s, e) =>
            {
                WindowTasks.WinSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
                WindowTasks.WinSetControlDefaultValues(editStkPnl, windowMetaList);
            });
            btnDelete.Click += new RoutedEventHandler((s, e) =>
            {
                DatabaseDataOps.DbDeleteRecord(windowMetaList, windowDataGrid);

                DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);

                WindowTasks.WinSetMode("CLEAR", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
                WindowTasks.WinSetControlDefaultValues(editStkPnl, windowMetaList);
            });
            btnExit.Click += new RoutedEventHandler(WindowTasks.WinClose);
            btnClear.Click += new RoutedEventHandler((s, e) =>
            {
                selectedFilter = 0;
                winFlt.SelectedIndex = selectedFilter;
                WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, false, windowMetaList, controlValues);
                WindowTasks.WinResetRecordSelector(tbSelectorText, tbOffset, tbFetch, windowMetaList);
                WindowTasks.WinSetMode("CLEAR", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
                WindowDataOps.WinClearControlDictionaryValues(controlValues);
                DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);

            });
            tbOffset.TextChanged += new TextChangedEventHandler((s, e) =>
            {
                DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, selectedFilter, controlValues, tbOffset, tbSelectorText);
            });

            btnNextPage.Click += new RoutedEventHandler((s, e) =>
              {
                  tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) + Convert.ToInt32(tbFetch.Text));
                  WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
              });

            btnPrevPage.Click += new RoutedEventHandler((s, e) =>
            {
                if (Convert.ToInt32(tbOffset.Text) >= Convert.ToInt32(tbFetch.Text))
                {
                    tbOffset.Text = Convert.ToString(Convert.ToInt32(tbOffset.Text) - Convert.ToInt32(tbFetch.Text));
                    WindowTasks.WinClearDataFields(winNew, editStkPnl, fltStkPnl, true, windowMetaList, controlValues);
                }
            });

            //Build up the layout with UI parts

            //Add the datagrid to the window
            Grid.SetColumn(dataGrid, 1);
            Grid.SetRow(dataGrid, 1);
            Grid.SetRowSpan(dataGrid, 1);

            mainGrid.Children.Add(dataGrid);

            //Add combo/text to stack panel
            fltStkPnl.Children.Add(labelFlt);
            fltStkPnl.Children.Add(winFlt);

            //Add Filter selector to main grid
            Grid.SetColumn(fltGrid, 1);
            Grid.SetRow(fltGrid, 0);
            mainGrid.Children.Add(fltGrid);

            //Add editing controls Stack Panel
            // editGrid.Children.Add(editStkPnl);

            Grid.SetColumn(editGrid, 0);
            Grid.SetRow(editGrid, 1);

            mainGrid.Children.Add(editGrid);

            //Add Editing buttons to stack pannel

            buttonStkPnl.Children.Add(btnSave);
            buttonStkPnl.Children.Add(btnNew);
            buttonStkPnl.Children.Add(btnDelete);
            buttonStkPnl.Children.Add(btnClear);
            buttonStkPnl.Children.Add(btnExit);

            //Add buttons to main grid
            Grid.SetColumn(buttonGrid, 0);
            Grid.SetColumnSpan(buttonGrid, 1);
            Grid.SetRow(buttonGrid, 2);
            mainGrid.Children.Add(buttonGrid);

            //Add Record selector to main grid
            Grid.SetColumn(recordSelectorGrid, 1);
            Grid.SetRow(recordSelectorGrid, 2);
            recordSelectorStkPnl.Children.Add(tbOffset);
            recordSelectorStkPnl.Children.Add(btnPrevPage);
            recordSelectorStkPnl.Children.Add(tbSelectorText);
            recordSelectorStkPnl.Children.Add(btnNextPage);
            recordSelectorStkPnl.Children.Add(tbFetch);
            mainGrid.Children.Add(recordSelectorGrid);

            //Add Message Area
            messageStkPnl.Children.Add(tbWinMode);
            Grid.SetColumn(messageGrid, 0);
            Grid.SetRow(messageGrid, 3);
            Grid.SetColumnSpan(messageGrid, 3);
            mainGrid.Children.Add(messageGrid);

            //Add Main Grid to window
            winNew.Content = mainGrid;


            //Prepare the form with data and set mode

            winFlt.SelectedIndex = 0;
            DatabaseDataOps.DbGetDataGridRows(winNew, windowMetaList, editStkPnl, fltStkPnl, windowDataGrid, 0, controlValues, tbOffset, tbSelectorText);
            WindowTasks.WinSetMode("CLEAR", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear, windowMetaList, tbWinMode);
            winNew.Show();
                        
        }
    }
}