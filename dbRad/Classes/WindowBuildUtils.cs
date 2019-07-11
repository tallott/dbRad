

using System;
using System.Windows;
using System.Windows.Controls;

namespace dbRad.Classes
{
    class WindowBuildUtils
    {
        public static void StackPanelInGrid(String stackStyle, String gridStyle, String borderStyle, StackPanel stackPanel, Grid grid)
        {
            stackPanel.Style = (Style)Application.Current.FindResource(stackStyle);
            Border border = new Border
            {
                Style = (Style)Application.Current.FindResource(borderStyle)
            };
            grid.Style = (Style)Application.Current.FindResource(gridStyle);
            grid.Children.Add(border);
            grid.Children.Add(stackPanel);
        }
        public static void DataGridInGrid(String dataGridStyle, String gridStyle, String borderStyle, DataGrid dataGrid, Grid grid)
        {
            dataGrid.Style = (Style)Application.Current.FindResource(dataGridStyle);
            Border border = new Border
            {
                Style = (Style)Application.Current.FindResource(borderStyle)
            };
            grid.Style = (Style)Application.Current.FindResource(gridStyle);
            grid.Children.Add(border);
            grid.Children.Add(dataGrid);
        }
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
    }
}
