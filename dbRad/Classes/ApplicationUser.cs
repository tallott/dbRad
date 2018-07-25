
namespace dbRad.Classes
{

    public partial class ApplicationUser
    {
        private static string _UserName = string.Empty;
        private static string _UserPassword = string.Empty;

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        public string UserPassword
        {
            get
            {
                return _UserPassword;
            }
            set
            {
                _UserPassword = value; 
            }
        }
    }
}
