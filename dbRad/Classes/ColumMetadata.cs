
namespace dbRad.Classes
{
    public partial class ColumMetadata
    {
        string _columnName = string.Empty;
        string _columnLabel = string.Empty;
        string _columnRowSource = string.Empty;
        string _columnFilter = string.Empty;
        string _columnOrderBy = string.Empty;
        string _columnType = string.Empty;
        string _columnEnabled = string.Empty;
        string _columnDefaultValue = string.Empty;
        string _columnRequiredValue = string.Empty;
        string _columnDescription = string.Empty;
        string _columnValue = string.Empty;


        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
            }
        }


        public string ColumnLabel
        {
            get
            {
                return _columnLabel;
            }
            set
            {
                _columnLabel = value;
            }
        }

        public string ColumnRowSource
        {
            get
            {
                return _columnRowSource;
            }
            set
            {
                _columnRowSource = value;
            }
        }

        public string ColumnFilter
        {
            get
            {
                return _columnFilter;
            }
            set
            {
                _columnFilter = value;
            }
        }

        public string ColumnOrderBy
        {
            get
            {
                return _columnOrderBy;
            }
            set
            {
                _columnOrderBy = value;
            }
        }

        public string ColumnType
        {
            get
            {
                return _columnType;
            }
            set
            {
                _columnType = value;
            }
        }

        public string ColumnEnabled
        {
            get
            {
                return _columnEnabled;
            }
            set
            {
                _columnEnabled = value;
            }
        }

        public string ColumnDefaultValue
        {
            get
            {
                return _columnDefaultValue;
            }
            set
            {
                _columnDefaultValue = value;
            }
        }

        public string ColumnRequiredValue
        {
            get
            {
                return _columnRequiredValue;
            }
            set
            {
                _columnRequiredValue = value;
            }
        }

        public string ColumnDescription
        {
            get
            {
                return _columnDescription;
            }
            set
            {
                _columnDescription = value;
            }
        }
        public string ColumnValue
        {
            get
            {
                return _columnValue;
            }
            set
            {
                _columnValue = value;
            }
        }
    }
}
