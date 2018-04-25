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
        public static Connections controlDb = new Connections();
        public static string controlDbFilePath = Env.ControlDbFilePath();

        public static Connections applicationlDb = new Connections();
        public static string applicationDbFilePath = Env.ApplicationDbFilePath();

        public static User applicationUser = new User();
        public static string userFilePath = Env.UserFilePath();

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
            window.Closed += new EventHandler((s, e) =>
                {
                    WindowTasks.ResetWinMain();
                });

            Grid mainGrid = new Grid();
            TabControl tabControl = new TabControl();

            TabItem settingstab = new TabItem();
            settingstab.Header = "Settings";
            settingstab.GotFocus += new RoutedEventHandler((s, e) =>
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            });

            TabItem controlDbtab = new TabItem();
            controlDbtab.Header = "Control";
            controlDbtab.GotFocus += new RoutedEventHandler((s, e) =>
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            });

            TabItem applicationDbtab = new TabItem();
            applicationDbtab.Header = "Application";
            applicationDbtab.GotFocus += new RoutedEventHandler((s, e) =>
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            });
            
            tabControl.Items.Add(settingstab);
            tabControl.Items.Add(controlDbtab);
            tabControl.Items.Add(applicationDbtab);
            
            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            RowDefinition row2 = new RowDefinition();
            row2.Height = GridLength.Auto;

            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);

            StackPanel controlDbStackPanel = new StackPanel();
            controlDbStackPanel.Style = FindStyle("winEditPanelStyle");

            controlDbtab.Content = controlDbStackPanel;

            StackPanel applicationDbStackPanel = new StackPanel();
            applicationDbStackPanel.Style = FindStyle("winEditPanelStyle");

            applicationDbtab.Content = applicationDbStackPanel;

            StackPanel settingsStackPanel = new StackPanel();
            settingsStackPanel.Style = FindStyle("winEditPanelStyle");

            settingstab.Content = settingsStackPanel;

            StackPanel buttonStackPanel = new StackPanel();
            buttonStackPanel.Style = FindStyle("winButtonStack");

            Style textBoxStyle = FindStyle("winTextBoxStyle");
            Style lableStyle = FindStyle("winLabelStyle");

            Button buttonClose = new Button();
            buttonClose.Name = "btnClose";
            buttonClose.Content = "Close";
            buttonClose.Style = FindStyle("winButtonStyle");
            buttonClose.Click += new RoutedEventHandler((s, e) =>
            {
                WindowTasks.winClose(s, e);
            });

            Button buttonSave = new Button();
            buttonSave.Name = "btnSave";
            buttonSave.Content = "Save";
            buttonSave.Style = FindStyle("winButtonStyle");
            buttonSave.Click += new RoutedEventHandler((s, e) =>
            {
                Filetasks.WriteToXmlFile<Connections>(controlDbFilePath, controlDb);
                Filetasks.WriteToXmlFile<Connections>(applicationDbFilePath, applicationlDb);
                Filetasks.WriteToXmlFile<User>(userFilePath, applicationUser);
                WindowTasks.winClose(s, e);

            });

            buttonStackPanel.Children.Add(buttonSave);
            buttonStackPanel.Children.Add(buttonClose);

            Grid.SetRow(controlDbStackPanel, 0);
            mainGrid.Children.Add(tabControl);

            if (File.Exists(controlDbFilePath))
            {
                controlDb = Filetasks.ReadFromXmlFile<Connections>(controlDbFilePath);
                BuildFormClass(controlDbStackPanel, lableStyle, textBoxStyle, controlDb, out controlDbStackPanel);
            }
            else
            {
                Filetasks.WriteToXmlFile<Connections>(controlDbFilePath, controlDb);
                BuildFormClass(controlDbStackPanel, lableStyle, textBoxStyle, controlDb, out controlDbStackPanel);
            }

            if (File.Exists(applicationDbFilePath))
            {
                applicationlDb = Filetasks.ReadFromXmlFile<Connections>(applicationDbFilePath);
                BuildFormClass(applicationDbStackPanel, lableStyle, textBoxStyle, applicationlDb, out applicationDbStackPanel);
            }
            else
            {
                Filetasks.WriteToXmlFile<Connections>(applicationDbFilePath, applicationlDb);
                BuildFormClass(applicationDbStackPanel, lableStyle, textBoxStyle, applicationlDb, out applicationDbStackPanel);
            }

            if (File.Exists(userFilePath))
            {
                applicationUser = Filetasks.ReadFromXmlFile<User>(userFilePath);
                BuildFormClass(settingsStackPanel, lableStyle, textBoxStyle, applicationUser, out settingsStackPanel);
            }
            else
            {
                Filetasks.WriteToXmlFile<User>(userFilePath, applicationUser);
                BuildFormClass(settingsStackPanel, lableStyle, textBoxStyle, applicationUser, out settingsStackPanel);
            }

            Grid.SetRow(buttonStackPanel, 1);
            mainGrid.Children.Add(buttonStackPanel);

            window.Content = mainGrid;
            window.SizeToContent = SizeToContent.WidthAndHeight;

        }
        private static StackPanel BuildFormClass<T>(StackPanel stackPanelIn, Style lableStyle, Style textBoxStyle, T Tclass, out StackPanel stackPanelOut)
        {
            Type tClass = Tclass.GetType();
            PropertyInfo[] tClassProperties = tClass.GetProperties();

            foreach (PropertyInfo tClassProperty in tClassProperties)
            {
                string propertyName = tClassProperty.Name.ToString();
                string propertyValue = tClassProperty.GetValue(Tclass, null).ToString();

                Label label = new Label();
                label.Style = lableStyle;
                label.Content = propertyName;

                TextBox textBox = new TextBox();
                textBox.Style = textBoxStyle;
                textBox.Name = propertyName;

                Binding binding = new Binding(propertyName);
                binding.Mode = BindingMode.TwoWay;
                binding.Source = Tclass;
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

