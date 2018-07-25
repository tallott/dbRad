
namespace dbRad.Classes
{
    public partial class ApplicationConnections
    {
        private string _HostName = string.Empty;
        private string _Name = string.Empty;
        private string _UserName = string.Empty;
        private string _UserPassword = "bv7gnE9AAL7g'kce8x)SHRf8!`Q&cd]3";

        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                _HostName = value;
            }
        }

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

        private string UserPassword
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

        public override string ToString()
        {
            //return "Data Source=" + _HostName + ";Initial Catalog=" + _Name + ";Persist Security Info=True;User ID=" + _UserName + ";Password=" + _UserPassword;
            return "Data Source=" + _HostName + ";Persist Security Info=True;User ID=" + _UserName + ";Password=" + _UserPassword;
        }
    }
}
