
namespace dbRad
{
    public partial class Connections
    {
        private string _HostName = string.Empty;
        private string _Name = string.Empty;
        private string _UserName = string.Empty;
        private string _UserPassword = "Pakula01";

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

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
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
            return "Data Source=" + _HostName + ";Initial Catalog=" + _Name + ";Persist Security Info=True;User ID=" + _UserName + ";Password=" + _UserPassword;
        }
    }
}
