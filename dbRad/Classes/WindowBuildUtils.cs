

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
            Border buttonGridBorder = new Border
            {
                Style = (Style)Application.Current.FindResource(borderStyle)
            };
            grid.Style = (Style)Application.Current.FindResource(gridStyle);
            grid.Children.Add(buttonGridBorder);
            grid.Children.Add(stackPanel);


        }
    }
}
