
namespace dbRad.Classes
{
    public partial class WindowMetaList
    {
        private static string _ApplicationName = string.Empty;
        private static string _TableName = string.Empty;
        private static string _TableLabel = string.Empty;
        private static string _SchemaName = string.Empty;
        private static string _SchemaLabel = string.Empty;
        private static string _TableKey = string.Empty;
        private static string _ApplicationTableId = string.Empty;

        public string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }

        public string TableLabel
        {
            get
            {
                return _TableLabel;
            }
            set
            {
                _TableLabel = value;
            }
        }

        public string SchemaName
        {
            get
            {
                return _SchemaName;
            }
            set
            {
                _SchemaName = value;
            }
        }

        public string SchemaLabel
        {
            get
            {
                return _SchemaLabel;
            }
            set
            {
                _SchemaLabel = value;
            }
        }

        public string TableKey
        {
            get
            {
                return _TableKey;
            }
            set
            {
                _TableKey = value;
            }
        }

        public string ApplicationTableId
        {
            get
            {
                return _ApplicationTableId;
            }
            set
            {
                _ApplicationTableId = value;
            }
        }
    }
}
