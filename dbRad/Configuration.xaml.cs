
using System.Windows;
using System.Data.SqlClient;



namespace dbRad
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Properties.Settings.Default.appDbCon);

            dataSource.Text = builder.DataSource;
            initialCatalog.Text = builder.InitialCatalog;
            userID.Text = builder.UserID;
            password.Password = builder.Password;
        }


    }
}
