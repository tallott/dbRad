

using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace dbRad.Classes
{
    class WindowBuildUtils
    {

        public static Window CreateWindow(WindowMetaList windowMetaList, Int32 applicationTableId)
        {
            Window window = new Window
            {
                Style = (Style)Application.Current.FindResource("winStyle"),
                Title = "Manage " + windowMetaList.TableLabel + " (" + windowMetaList.TableName + ")",
                Name = windowMetaList.TableName
            };
            window.Activated += new EventHandler((s, e) =>
            {
                windowMetaList = WindowTasks.WinMetadataList(applicationTableId);
            });

            return window;
        }
        public static Grid CreateMainGrid()
        {

            Grid grid = new Grid();

            //1st Column
            ColumnDefinition col1 = new ColumnDefinition
            {
                Width = GridLength.Auto
            };

            //2nd Column
            ColumnDefinition col2 = new ColumnDefinition();
            col1.Width = GridLength.Auto;

            //1st Row
            RowDefinition row1 = new RowDefinition
            {
                Height = GridLength.Auto
            };

            //2nd row
            RowDefinition row2 = new RowDefinition
            {
                Height = GridLength.Auto
            };

            //3rd row
            RowDefinition row3 = new RowDefinition
            {
                Height = GridLength.Auto
            };
            //4th row
            RowDefinition row4 = new RowDefinition
            {
                Height = GridLength.Auto
            };

            //Add Rows and Columns to the Grid
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);

            return grid;
        }
        public static StackPanel CreateStackPanelInGrid(String stackStyle, String gridStyle, String borderStyle, Grid grid)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Style = (Style)Application.Current.FindResource(stackStyle);
            Border border = new Border
            {
                Style = (Style)Application.Current.FindResource(borderStyle)
            };
            grid.Style = (Style)Application.Current.FindResource(gridStyle);
            grid.Children.Add(border);
            grid.Children.Add(stackPanel);
            return stackPanel;
        }
        public static DataGrid CreateDataGridInGrid(String dataGridStyle, String gridStyle, String borderStyle, Grid grid)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Style = (Style)Application.Current.FindResource(dataGridStyle);
            Border border = new Border
            {
                Style = (Style)Application.Current.FindResource(borderStyle)
            };
            grid.Style = (Style)Application.Current.FindResource(gridStyle);
            grid.Children.Add(border);
            grid.Children.Add(dataGrid);

            return dataGrid;
        }

        public static Label CreateLabel(string labelContent, string controlStyle)
        {
            Label label = new Label
            {
                Content = labelContent,
                Style = (Style)Application.Current.FindResource(controlStyle)
            };

            return label;
        }
        public static Button CreateButton(string controlName, string buttonContent, string controlStyle)
        {
            Button button = new Button
            {
                Name = controlName,
                Content = buttonContent,
                Style = (Style)Application.Current.FindResource(controlStyle)
            };

            return button;
        }

        public static TextBox CreateTextBox(string controlName, string textBoxText, string controlStyle, Visibility controlVisibility)
        {
            TextBox textBox = new TextBox
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource(controlStyle),
                Visibility = controlVisibility,
                Text = textBoxText
            };

            return textBox;
        }
        public static TextBox CreateTextBox(string controlName, string controlStyle, string controlEnabled, string controlType, string textBoxToolTip)
        {
            TextBox textBox = new TextBox
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource(controlStyle),
                IsEnabled = Convert.ToBoolean(controlEnabled),
                Tag = controlType,
                ToolTip = textBoxToolTip
            };

            return textBox;
        }
        public static TextBox CreateTextBox(string controlName, string controlStyle, string controlEnabled, string controlType, string textBoxToolTip, WindowMetaList windowMetaList )
        {
            List<ColumMetadata> columns = new List<ColumMetadata>();
            columns = windowMetaList.Columns;

            MultiBinding multiBinding = new MultiBinding();
            //multiBinding.Converter = "";

            TextBox textBox = new TextBox
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource(controlStyle),
                IsEnabled = Convert.ToBoolean(controlEnabled),
                Tag = controlType,
                ToolTip = textBoxToolTip,
                DataContext = "windowMetaList.Columns",
                
            };
            textBox.SetBinding(TranslateTransform.YProperty, multiBinding);
            return textBox;
        }
        public static ComboBox CreateComboBox(string controlName, string controlStyle, string controlEnabled, string controlType, string controlToolTip)
        {
            ComboBox comboBox = new ComboBox
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource("winComboBoxStyle"),
                Tag = controlType,
                IsEnabled = Convert.ToBoolean(controlEnabled),
                SelectedValuePath = "value_member",
                DisplayMemberPath = "display_member",
                ToolTip = controlToolTip
            };
            return comboBox;
        }
        public static ComboBox CreateFilterCombo(WindowMetaList windowMetaList)
        {
            //Window Filter - Gets the list of filters for the window based on the underlying database table

            ComboBox winFlt = new ComboBox
            {
                Name = "gridFilter",
                Style = (Style)Application.Current.FindResource("winComboBoxStyle")
            };
            NpgsqlCommand getFltRows = new NpgsqlCommand
            {
                CommandText = ControlDatabaseSql.TableFilterList()
            };

            getFltRows.Parameters.AddWithValue("@applicationTableId", windowMetaList.TableId);
            getFltRows.CommandType = CommandType.Text;
            getFltRows.Connection = windowMetaList.ControlDb;

            windowMetaList.ControlDb.Open();
            {
                NpgsqlDataAdapter fltAdapter = new NpgsqlDataAdapter(getFltRows);
                DataTable fltDataTable = new DataTable();
                fltAdapter.Fill(fltDataTable);
                winFlt.ItemsSource = fltDataTable.DefaultView;
                winFlt.DisplayMemberPath = fltDataTable.Columns["display_member"].ToString();
                winFlt.SelectedValuePath = fltDataTable.Columns["value_member"].ToString();
            }
            windowMetaList.ControlDb.Close();
            return winFlt;
        }
        public static CheckBox CreateCheckBox(string controlName, string controlStyle, string controlEnabled, string controlType, string checkBoxToolTip)
        {
            CheckBox checkBox = new CheckBox
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource(controlStyle),
                Tag = controlType,
                IsEnabled = Convert.ToBoolean(controlEnabled),
                ToolTip = checkBoxToolTip
            };
            return checkBox;
        }

        public static DatePicker CreateDatePicker(string controlName, string controlStyle, string controlEnabled, string controlType, string datePickerToolTip)
        {
            DatePicker datePicker = new DatePicker
            {
                Name = controlName,
                Style = (Style)Application.Current.FindResource(controlStyle),
                Tag = controlType,
                IsEnabled = Convert.ToBoolean(controlEnabled),
                ToolTip = datePickerToolTip
            };
            return datePicker;
        }

        public static List<ColumMetadata> PopulateColumnMetadata(WindowMetaList windowMetaList)
        {
            List<ColumMetadata> columns = new List<ColumMetadata>();

            NpgsqlCommand getColList = new NpgsqlCommand
            {
                CommandText = ControlDatabaseSql.ColumnMetadata()
            };

            getColList.Parameters.AddWithValue("@applicationTableId", windowMetaList.TableId);
            getColList.CommandType = CommandType.Text;
            getColList.Connection = windowMetaList.ControlDb;

            windowMetaList.ControlDb.Open();

            NpgsqlDataReader getColListReader = getColList.ExecuteReader();
            if (getColListReader.HasRows)
            {
                while (getColListReader.Read())
                {
                    ColumMetadata column = new ColumMetadata
                    {
                        ColumnName = getColListReader["column_name"].ToString(),
                        ColumnLabel = getColListReader["column_label"].ToString(),
                        ColumnRowSource = getColListReader["row_source"].ToString(),
                        ColumnFilter = getColListReader["filter"].ToString(),
                        ColumnOrderBy = getColListReader["order_by"].ToString(),
                        ColumnType = getColListReader["window_control_type"].ToString(),
                        ColumnEnabled = getColListReader["window_control_enabled"].ToString(),
                        ColumnDefaultValue = getColListReader["column_default_value"].ToString(),
                        ColumnRequiredValue = getColListReader["column_required_value"].ToString(),
                        ColumnDescription = getColListReader["column_description"].ToString(),
                        ColumnUiValue = String.Empty
                    };

                    columns.Add(column);
                }
            }
            windowMetaList.ControlDb.Close();

            return columns;
        }
    }
}
