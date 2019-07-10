
using Npgsql;
using System;
using System.Data;

namespace dbRad.Classes
{
    public partial class WindowMetaList
    {
        private static string _ApplicationName = string.Empty;
        private static string _TableName = string.Empty;
        private static string _TableLabel = string.Empty;
        private static string _TableDml = string.Empty;
        private static string _TableOrderBy = string.Empty;
        private static string _TableFilter = string.Empty;
        private static string _PageRowCount = string.Empty;
        private static string _SchemaName = string.Empty;
        private static string _SchemaLabel = string.Empty;
        private static string _TableKey = string.Empty;
        private static string _WinMode = string.Empty;
        private static DataTable _controlValues;
        private static Int32 _GridSelectedIndex = -1;
        private static Int32 _TableId = 0;
        private static NpgsqlConnection _controlDb;
        private static NpgsqlConnection _applicationDb;

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

        public string TableDml
        {
            get
            {
                return _TableDml;
            }
            set
            {
                _TableDml = value;
            }
        }

        public string TableOrderBy
        {
            get
            {
                return _TableOrderBy;
            }
            set
            {
                _TableOrderBy = value;
            }
        }

        public string TableFilter
        {
            get
            {
                return _TableFilter;
            }
            set
            {
                _TableFilter = value;
            }
        }

        public string PageRowCount
        {
            get
            {
                return _PageRowCount;
            }
            set
            {
                _PageRowCount = value;
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

        public string WinMode
        {
            get
            {
                return _WinMode;
            }
            set
            {
                _WinMode = value;
            }
        }
        public DataTable ControlValues
        {
            get
            {
                return _controlValues;
            }
            set
            {
                _controlValues = value;
            }
        }
        public Int32 GridSelectedIndex
        {
            get
            {
                return _GridSelectedIndex;
            }
            set
            {
                _GridSelectedIndex = value;
            }
        }

        public Int32 TableId
        {
            get
            {
                return _TableId;
            }
            set
            {
                _TableId = value;
            }
        }

        public NpgsqlConnection ControlDb
        {
            get
            {
                return _controlDb;
            }
            set
            {
                _controlDb = value;
            }
        }
        public NpgsqlConnection ApplicationDb
        {
            get
            {
                return _applicationDb;
            }
            set
            {
                _applicationDb = value;
            }
        }
    }
}
