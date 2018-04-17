﻿using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;


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
            appStartup();


        }
        private void appStartup()
        {
            if (File.Exists(Config.controlDbFilePath))
            {
                Config.controlDb = Filetasks.ReadFromXmlFile<Connections>(Config.controlDbFilePath);
            }
            if (File.Exists(Config.applicationDbFilePath))
            {
                Config.applicationlDb = Filetasks.ReadFromXmlFile<Connections>(Config.applicationDbFilePath);
            }
            if (Config.controlDb.HostName == string.Empty)
            {
                Window config = new Config();
                config.Show();
            }
            else
            {
                mainWinBuild();
            }

        }

        private void appShutdown(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
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

        private void dbGetDataGridRows(Window winNew, String tabId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, Int32 selectedFilter, Dictionary<string, string> columnValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Fills the form data grid with the filter applied
        {
            SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());

            DataTable winDt = new DataTable();

            string sqlPart;
            string sqlParam = tabId;

            //Single row to return user defined DML SQL for DataGrid
            sqlPart = "SELECT TOP 1 Dml from ApplicationTable WHERE ApplicationTableId = @sqlParam";

            string sqlTxt = dataGridGetBaseSql(sqlPart, sqlParam);

            //Append filter where clause to the end of DML
            if (selectedFilter == 0) //Default filter selected
            {
                sqlPart = "SELECT FilterDefinition FROM ApplicationFilter WHERE ApplicationTableId = @sqlparam AND SortOrder = 1";
            }
            else //Custom filter selected
            {
                sqlParam = selectedFilter.ToString();
                sqlPart = "SELECT FilterDefinition FROM ApplicationFilter WHERE ApplicationFilterId = @sqlparam";
            }

            string fltTxt = dataGridGetBaseSql(sqlPart, sqlParam);

            winNew.Resources.Remove("winFilter");
            winNew.Resources.Add("winFilter", fltTxt);

            //Build where clause with replacement values for $COLUMN_NAME$ parameters  
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
                if (fltTxt.Contains(s))
                {
                    fltTxt = fltTxt.Replace(s, r);
                    //Set the window column value here
                    if (r != "''")
                        winLoadFilterValues(editStkPnl, s, r);
                }


            }

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
                        string tabSchema = WindowTasks.winMetadataList(tabId)[2];
                        string tabName = WindowTasks.winMetadataList(tabId)[0];
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
                MessageBox.Show("ERROR in Filter SQL: (" + sqlTxt + ") :" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                appDbCon.Close();
            }
        }

        private string dataGridGetBaseSql(string sqlpart, string sqlParam)
        //Single row to return user defined DML SQL for DataGrid
        {

            SqlConnection controlDbCon = new SqlConnection(Config.controlDb.ToString());

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
                MessageBox.Show("Problem Selecting the DataGrid Row :" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string dataGridGetId(DataGrid winDg)
        //Gets the Id of the selected grid row
        {
            string id = null;

            DataRowView drv = (DataRowView)winDg.SelectedValue;
            id = drv.Row.ItemArray[0].ToString();
            return id;


        }

        private void dataGridClicked(String tabId, DataGrid winDg, StackPanel editStkPnl, Dictionary<string, string> controlValues)
        //gets the id of the row selected and loads the edit fileds with the database values
        {
            string id = dataGridGetId(winDg);
            try
            {

                if (id != null)
                {
                    DataTable winSelectedRowDataTable = dbGetDataRow(tabId, id, editStkPnl);
                    winLoadDataRow(editStkPnl, winSelectedRowDataTable, controlValues);
                    winDg.UpdateLayout();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem Loading data:" + ex.Message + ex.StackTrace, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void winLoadFilterValues(StackPanel editStkPnl, string editColumn, string filterValue)
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

        private void winLoadDataRow(StackPanel editStkPnl, DataTable winSelectedRowDataTable, Dictionary<string, string> controlValues)
        //Loads the data editing UI with the values from the row in winSelectedRowDataTable 
        {
            //Loop the Row (Filtered by @Id) and columns of the underlying dataset
            try
            {

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    foreach (DataColumn col in winSelectedRowDataTable.Columns)
                    {
                        //Set the value of the control col.Name in the window to the value returned by row[col]

                        //Determine the Type of control
                        object obj = editStkPnl.FindName(col.ColumnName);

                        string ctlType = obj.GetType().Name;
                        //Use Type to work out how to process value;

                        switch (ctlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)editStkPnl.FindName(col.ColumnName);
                                tb.Text = row[col].ToString();
                                break;

                            case "ComboBox":
                                ComboBox cb = (ComboBox)editStkPnl.FindName(col.ColumnName);
                                cb.SelectedValue = row[col].ToString();
                                //We set this here because there is no change event we can trigger on a combo box
                                winGetControlValue(cb, controlValues);

                                break;

                            case "DatePicker":
                                DatePicker dtp = (DatePicker)editStkPnl.FindName(col.ColumnName);
                                if (row[col].ToString() != "")
                                {
                                    dtp.SelectedDate = Convert.ToDateTime(row[col]);
                                }
                                else if (row[col].ToString() == "")
                                {
                                    dtp.SelectedDate = null;
                                }

                                break;

                            case "CheckBox":
                                CheckBox chk = (CheckBox)editStkPnl.FindName(col.ColumnName);
                                chk.IsChecked = Convert.ToBoolean(row[col]);
                                break;
                        };
                    }
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Problem Loading the data row :" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private DataTable dbGetDataRow(string tabId, string id, StackPanel editStkPnl)
        //Loads a single row from the database into a table for the record for the selected ID
        {
            SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());

            string tabSchema = WindowTasks.winMetadataList(tabId)[2];
            string tabName = WindowTasks.winMetadataList(tabId)[0];
            string tabKey = WindowTasks.winMetadataList(tabId)[3];

            DataTable winSelectedRowDataTable = new DataTable();



            SqlCommand winSelectedRowSql = new SqlCommand();
            winSelectedRowSql.CommandText = "SELECT * FROM " + tabSchema + ".[" + tabName + "] WHERE " + tabKey + " = @Id";
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
                MessageBox.Show("Problem Loading data grid row:" + winSelectedRowSql + ":" + ex.Message + ex.StackTrace, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        ;

            appDbCon.Close();
            return winSelectedRowDataTable;

        }

        private void dbCreateRecord(Window winNew, String tabId, StackPanel editStkPnl, StackPanel fltStkPnl, DataGrid winDg, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //Creates a new record in the db
        {
            try
            {
                SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());

                string tabSchema = WindowTasks.winMetadataList(tabId)[2];
                string tabName = WindowTasks.winMetadataList(tabId)[0];
                string tabKey = WindowTasks.winMetadataList(tabId)[3];


                List<string> columns = new List<string>();
                List<string> columnUpdates = new List<string>();

                foreach (FrameworkElement element in editStkPnl.Children)
                {
                    if (element.Name != tabKey)
                    {

                        string ctlType = element.GetType().Name;
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
                string sql = "INSERT INTO " + tabSchema + "." + tabName + csvColumns + csvColumnUpdates;

                SqlCommand dbCreateRecordSql = new SqlCommand();
                dbCreateRecordSql.CommandText = sql;
                dbCreateRecordSql.CommandType = CommandType.Text;
                dbCreateRecordSql.Connection = appDbCon;

                appDbCon.Open();

                dbCreateRecordSql.ExecuteNonQuery();

                appDbCon.Close();

                dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Insert Record:" + ex.Message + ex.StackTrace, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dbUpdateRecord(Window winNew, String tabId, DataGrid winDg, StackPanel editStkPnl, StackPanel fltStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //updates the database with values in the data edit fields
        {
            SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());


            string tabSchema = WindowTasks.winMetadataList(tabId)[2];
            string tabName = WindowTasks.winMetadataList(tabId)[0];
            string tabKey = WindowTasks.winMetadataList(tabId)[3];

            try
            {
                string id = dataGridGetId(winDg);

                DataTable winSelectedRowDataTable = dbGetDataRow(tabId, id, editStkPnl);

                Boolean isDirty = false;

                foreach (DataRow row in winSelectedRowDataTable.Rows)
                {
                    string sql = "UPDATE " + tabSchema + "." + tabName + " SET ";
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

                        sql = sql.Trim(',', ' ') + " WHERE " + tabKey + " = @Id"; ;

                        SqlCommand listItemSaveSql = new SqlCommand();
                        listItemSaveSql.CommandText = sql;
                        listItemSaveSql.Parameters.AddWithValue("@Id", id);
                        listItemSaveSql.CommandType = CommandType.Text;
                        listItemSaveSql.Connection = appDbCon;

                        appDbCon.Open();
                        try
                        {
                            listItemSaveSql.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in SQL" + sql + ";  " + ex.Message + ex.StackTrace, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        appDbCon.Close();

                        dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

                        TextBox tabIdCol = (TextBox)editStkPnl.FindName(tabKey);
                        id = tabIdCol.Text;

                        dataGridSelectRow(id, winDg);

                    };


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Save Record:" + ex.Message + ex.StackTrace, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                appDbCon.Close();
            };
        }

        private void dbDeleteRecord(Window winNew, String tabId, StackPanel fltStkPnl, DataGrid winDg, StackPanel editStkPnl, int seletedFilter, Dictionary<string, string> controlValues, TextBox tbOffset, TextBox tbFetch, TextBox tbSelectorText)
        //deletes the selected row from the database
        {
            SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());

            string tabSchema = WindowTasks.winMetadataList(tabId)[2];
            string tabName = WindowTasks.winMetadataList(tabId)[0];
            string tabKey = WindowTasks.winMetadataList(tabId)[3];

            try
            {
                string id = dataGridGetId(winDg);
                DataTable winSelectedRowDataTable = dbGetDataRow(tabId, id, editStkPnl);
                //Delete the selected row from db

                SqlCommand delRowSql = new SqlCommand();
                delRowSql.CommandText = "DELETE FROM " + tabSchema + "." + tabName + " WHERE " + tabKey + " = @Id";
                delRowSql.Parameters.AddWithValue("@Id", id);
                delRowSql.CommandType = CommandType.Text;
                delRowSql.Connection = appDbCon;

                appDbCon.Open();
                delRowSql.ExecuteNonQuery();
                appDbCon.Close();

                winClearDataFields(winNew, editStkPnl, fltStkPnl, false);

                dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Delete Record:" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                appDbCon.Close();
            };
        }

        private void mainWinBuild()

        //Builds the main window for the appDbName
        {
            SqlConnection ctrlDbCon = new SqlConnection(Config.controlDb.ToString());

            //Get configured Application database
            string appDbName = Config.applicationlDb.Name;
            this.Title = appDbName;

            //Build Main Menu

            //A File Menu
            MenuItem fileMenu = new MenuItem();
            fileMenu.Header = "File";
            this.menu.Items.Add(fileMenu);

            //Item 1 Exit
            MenuItem fileExitItem = new MenuItem();
            fileExitItem.Header = "Exit";
            fileExitItem.Click += new RoutedEventHandler(appShutdown);
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

            //Open Menu Items are Dynamicly populated with table names in control.ApplicationTable;
            SqlCommand getTabList = new SqlCommand();

            //x
            //Edit this sql to filter to user role assigned tables only
            getTabList.CommandText = "SELECT t.ApplicationTableId, t.TableLabel FROM ApplicationTable t INNER JOIN ApplicationSchema apps ON t.ApplicationSchemaId = apps.ApplicationSchemaId INNER JOIN Application a on apps.ApplicationId = a.ApplicationId WHERE a.ApplicationName = @appDbName ORDER BY t.TableName";
            getTabList.CommandType = CommandType.Text;
            getTabList.Parameters.AddWithValue("@appDbName", appDbName);
            getTabList.Connection = ctrlDbCon;
            ctrlDbCon.Open();
            {
                SqlDataReader reader = getTabList.ExecuteReader();
                while (reader.Read())
                {
                    string tabId = reader["ApplicationTableId"].ToString();
                    string tabLable = reader["TableLabel"].ToString();
                    MenuItem fileOpenItem = new MenuItem();
                    fileOpenItem.Header = tabLable;
                    fileOpenItem.Click += new RoutedEventHandler((s, e) => { winConstruct(tabId); });

                    openMenu.Items.Add(fileOpenItem);
                };
            }
            ctrlDbCon.Close();
        }

        private void winGetControlValue(ComboBox cb, Dictionary<string, string> controlValues)

        {
            controlValues[cb.Name] = cb.SelectedValue.ToString();
        }

        private void winGetControlValue(TextBox tb, Dictionary<string, string> controlValues)

        {
            controlValues[tb.Name] = "'" + tb.Text + "'";
        }

        private void winGetControlValue(CheckBox cb, Dictionary<string, string> controlValues)

        {
            controlValues[cb.Name] = cb.IsChecked.ToString();
        }

        private void winGetControlValue(DatePicker dtp, Dictionary<string, string> controlValues)

        {
            controlValues[dtp.Name] = "'" + dtp.Text + "'";
        }

        private void winClearControlDictionaryValues(Dictionary<string, string> controlValues)
        //Clears the list of window values used for filters
        {
            foreach (string key in controlValues.Keys.ToList())
            {
                controlValues[key] = "";
            }
        }

        private void winConstruct(string tabId)
        //Builds the window for the selected table 
        {
            SqlConnection appDbCon = new SqlConnection(Config.applicationlDb.ToString());
            SqlConnection ctrlDbCon = new SqlConnection(Config.controlDb.ToString());

            string controlName;
            string controlLabel;
            string controlRowSource;
            string controlType;
            string controlEnabled;
            Dictionary<string, string> controlValues = new Dictionary<string, string>();
            Int32 seletedFilter = 0;

            string tabName = WindowTasks.winMetadataList(tabId)[0];
            string tableLabel = WindowTasks.winMetadataList(tabId)[1];
            string tableSchema = WindowTasks.winMetadataList(tabId)[2];
            string TableKey = WindowTasks.winMetadataList(tabId)[3];

            //Create a new window - this is a window based on an underlying database table
            Window winNew = new Window();
            winNew.Style = (Style)FindResource("winStyle");
            winNew.Title = "Manage " + tableLabel + " (" + tabName + ")";
            winNew.Name = tabName;

            winNew.Resources.Add("tabId", tabId);
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
            getFltRows.CommandText = "SELECT ApplicationFilterId as valueMember, FilterName as displayMember FROM ApplicationFilter WHERE ApplicationTableId =  @tabId ORDER BY SortOrder";

            getFltRows.Parameters.AddWithValue("@tabId", tabId);
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
            getColList.CommandText = "SELECT c.ColumnName, ISNULL(c.ColumnLable,c.ColumnName) AS ColumnLabel, c.RowSource, ct.WindowControlType, c.WindowControlEnabled FROM ApplicationColumn c INNER JOIN WindowControlType ct on c.WindowControlTypeId = ct.WindowControlTypeId WHERE ApplicationTableId =  @tabId ORDER BY c.WindowLayoutOrder";

            getColList.Parameters.AddWithValue("@tabId", tabId);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = ctrlDbCon;

            ctrlDbCon.Open();
            {
                SqlDataReader getColListReader = getColList.ExecuteReader();
                while (getColListReader.Read())
                {
                    controlName = getColListReader["ColumnName"].ToString();
                    controlLabel = getColListReader["ColumnLabel"].ToString();
                    controlRowSource = getColListReader["RowSource"].ToString();
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
                                winGetControlValue(tb, controlValues);
                            });
                            break;

                        case "TEXTBLOCK":
                            TextBox tbk = new TextBox();
                            tbk.Name = controlName;
                            tbk.Style = (Style)FindResource("winTextBlockStyle");
                            tbk.IsEnabled = Convert.ToBoolean(controlEnabled);
                            editStkPnl.Children.Add(tbk);
                            editStkPnl.RegisterName(tbk.Name, tbk);
                            tbk.TextChanged += new TextChangedEventHandler((s, e) =>
                            {
                                winGetControlValue(tbk, controlValues);
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
                                winGetControlValue(nb, controlValues);
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
                                winGetControlValue(chk, controlValues);
                            });
                            chk.Unchecked += new RoutedEventHandler((s, e) =>
                            {
                                winGetControlValue(chk, controlValues);
                            });
                            chk.Indeterminate += new RoutedEventHandler((s, e) =>
                            {
                                winGetControlValue(chk, controlValues);
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
                            cb.Name = controlName;
                            cb.Style = (Style)FindResource("winComboBoxStyle");
                            cb.IsEnabled = Convert.ToBoolean(controlEnabled);
                            cb.SelectedValuePath = "valueMember";
                            cb.DisplayMemberPath = "displayMember";
                            cb.DropDownClosed += new EventHandler((s, e) =>
                            {
                                if (cb.SelectedValue != null)
                                {
                                    winGetControlValue(cb, controlValues);
                                    if (winNew.Resources["winMode"].ToString() != "EDIT")
                                    {
                                        dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                                    }

                                }
                            });
                            //Populate Combo
                            SqlCommand getComboRows = new SqlCommand();
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
            ctrlDbCon.Close();

            //Event Handler's

            //Data Grid
            winDg.SelectionChanged += new SelectionChangedEventHandler((s, e) =>
            {
                if (winDg.SelectedItem == null) return;

                WindowTasks.winSetMode("EDIT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                dataGridClicked(tabId, winDg, editStkPnl, controlValues);
            });

            //Filter Selector
            winFlt.DropDownClosed += new EventHandler((s, e) =>
            {
                ComboBox clicked = (ComboBox)s;
                seletedFilter = (Int32)clicked.SelectedValue;
                WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                winResetRecordSelector(tbSelectorText, tbOffset, tbFetch);
                dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
            }
            );

            //buttons
            btnSave.Click += new RoutedEventHandler((s, e) =>
            {
                switch (winNew.Resources["winMode"].ToString())
                {
                    case "NEW":
                        dbCreateRecord(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
                        winClearDataFields(winNew, editStkPnl, fltStkPnl, true);
                        WindowTasks.winSetMode("NEW", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
                        break;
                    case "EDIT":
                        dbUpdateRecord(winNew, tabId, winDg, editStkPnl, fltStkPnl, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
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
                dbDeleteRecord(winNew, tabId, fltStkPnl, winDg, editStkPnl, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
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
                winClearControlDictionaryValues(controlValues);
                dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);

            });
            tbOffset.TextChanged += new TextChangedEventHandler((s, e) =>
            {
                dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, seletedFilter, controlValues, tbOffset, tbFetch, tbSelectorText);
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


            dbGetDataGridRows(winNew, tabId, editStkPnl, fltStkPnl, winDg, 0, controlValues, tbOffset, tbFetch, tbSelectorText);
            WindowTasks.winSetMode("SELECT", winNew, btnSave, btnNew, btnDelete, btnExit, btnClear);
        }
    }
}