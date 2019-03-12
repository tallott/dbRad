using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace dbRad.Classes
{
    class ApplicationUtils
    {
        public static void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        //Makes sure users can only enter numerics
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static void AppShutdown(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
