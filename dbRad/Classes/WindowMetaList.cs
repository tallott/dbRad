﻿
using Npgsql;
using System;

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
        
        public NpgsqlConnection controlDb
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
        public NpgsqlConnection applicationDb
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
