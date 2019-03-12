

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
    }
}
