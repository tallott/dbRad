﻿using dbRad.Classes;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace dbRad
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public Config()
        {
            InitializeComponent();
            InitSettings();
        }
        public static ApplicationConnections appDb = new ApplicationConnections();
        public static string appDbFilePath = ApplicationEnviroment.ApplicationDbFilePath();

        public static ApplicationUser applicationUser = new ApplicationUser();
        public static string userFilePath = ApplicationEnviroment.UserFilePath();

        private Style FindStyle(string styleName)
        {
            Style style = (Style)FindResource(styleName);
            return style;
        }

        private void InitSettings()
        {
            Window window = this;
            window.Name = "winConfig";
            window.Style = FindStyle("winStyle");
            //window.Closed += new EventHandler((s, e) =>
            //    {
            //        WindowTasks.ResetWinMain();
            //    });

            Grid mainGrid = new Grid();
            TabControl tabControl = new TabControl();

            TabItem logintab = new TabItem
            {
                Header = "User Login"
            };
            logintab.GotFocus += new RoutedEventHandler((s, e) =>
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            });

            TabItem appDbTab = new TabItem
            {
                Header = "Application Host"
            };
            appDbTab.GotFocus += new RoutedEventHandler((s, e) =>
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            });

            tabControl.Items.Add(logintab);
            tabControl.Items.Add(appDbTab);

            RowDefinition row1 = new RowDefinition
            {
                Height = GridLength.Auto
            };

            RowDefinition row2 = new RowDefinition
            {
                Height = GridLength.Auto
            };

            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);

            StackPanel controlDbStackPanel = new StackPanel
            {
                Style = FindStyle("winEditPanelStyle")
            };

            appDbTab.Content = controlDbStackPanel;

            StackPanel settingsStackPanel = new StackPanel
            {
                Style = FindStyle("winEditPanelStyle")
            };

            logintab.Content = settingsStackPanel;

            StackPanel buttonStackPanel = new StackPanel
            {
                Style = FindStyle("winButtonStack")
            };

            Style textBoxStyle = FindStyle("winTextBoxStyle");
            Style labelStyle = FindStyle("winLabelStyle");

            Button buttonClose = new Button
            {
                Name = "btnClose",
                Content = "Close",
                Style = FindStyle("winButtonStyle")
            };
            buttonClose.Click += new RoutedEventHandler((s, e) =>
            {
                WindowTasks.WinClose(s, e);
            });

            Button buttonSave = new Button
            {
                Name = "btnSave",
                Content = "Save",
                Style = FindStyle("winButtonStyle")
            };
            buttonSave.Click += new RoutedEventHandler((s, e) =>
            {
                ApplicationFiletasks.WriteToXmlFile<ApplicationConnections>(appDbFilePath, appDb);
                ApplicationFiletasks.WriteToXmlFile<ApplicationUser>(userFilePath, applicationUser);
                WindowTasks.WinClose(s, e);

            });

            Button buttonCancel = new Button
            {
                Name = "btnCancel",
                Content = "Cancel",
                Style = FindStyle("winButtonStyle")
            };
            buttonCancel.Click += new RoutedEventHandler((s, e) =>
            {
                if (Config.appDb.HostName == string.Empty || Config.applicationUser.UserName == string.Empty)
                {
                    ApplicationUtils.AppShutdown(s, e);
                }
                else
                {
                    WindowTasks.WinClose(s, e);
                }

            });

            buttonStackPanel.Children.Add(buttonSave);
            buttonStackPanel.Children.Add(buttonClose);
            buttonStackPanel.Children.Add(buttonCancel);

            Grid.SetRow(controlDbStackPanel, 0);
            mainGrid.Children.Add(tabControl);

            if (File.Exists(appDbFilePath))
            {
                appDb = ApplicationFiletasks.ReadFromXmlFile<ApplicationConnections>(appDbFilePath);
                BuildFormClass(controlDbStackPanel, labelStyle, textBoxStyle, appDb, out controlDbStackPanel);
            }
            else
            {
                ApplicationFiletasks.WriteToXmlFile<ApplicationConnections>(appDbFilePath, appDb);
                BuildFormClass(controlDbStackPanel, labelStyle, textBoxStyle, appDb, out controlDbStackPanel);
            }

            if (File.Exists(userFilePath))
            {
                applicationUser = ApplicationFiletasks.ReadFromXmlFile<ApplicationUser>(userFilePath);
                BuildFormClass(settingsStackPanel, labelStyle, textBoxStyle, applicationUser, out settingsStackPanel);
            }
            else
            {
                ApplicationFiletasks.WriteToXmlFile<ApplicationUser>(userFilePath, applicationUser);
                BuildFormClass(settingsStackPanel, labelStyle, textBoxStyle, applicationUser, out settingsStackPanel);
            }

            Grid.SetRow(buttonStackPanel, 1);
            mainGrid.Children.Add(buttonStackPanel);

            window.Content = mainGrid;
            window.SizeToContent = SizeToContent.WidthAndHeight;

        }
        private static StackPanel BuildFormClass<T>(StackPanel stackPanelIn, Style labelStyle, Style textBoxStyle, T Tclass, out StackPanel stackPanelOut)
        {
            Type tClass = Tclass.GetType();
            PropertyInfo[] tClassProperties = tClass.GetProperties();

            foreach (PropertyInfo tClassProperty in tClassProperties)
            {
                string propertyName = tClassProperty.Name.ToString();
                string propertyValue = tClassProperty.GetValue(Tclass, null).ToString();

                Label label = new Label
                {
                    Style = labelStyle,
                    Content = propertyName
                };

                TextBox textBox = new TextBox
                {
                    Style = textBoxStyle,
                    Name = propertyName
                };

                Binding binding = new Binding(propertyName)
                {
                    Mode = BindingMode.TwoWay,
                    Source = Tclass
                };
                textBox.SetBinding(TextBox.TextProperty, binding);

                textBox.TextChanged += new TextChangedEventHandler((s, e) =>
                {
                    ((TextBox)s).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
                );

                stackPanelIn.Children.Add(label);
                stackPanelIn.Children.Add(textBox);

            }

            stackPanelOut = stackPanelIn;
            return stackPanelOut;
        }


    }
}

