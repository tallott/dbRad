using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbRad.Classes
{
    partial class ControlDatabaseSql
    {

        public static string ApplicationList()
        {
            string _ApplicationList =
            @"SELECT DISTINCT
                        application_id,
                        application_name
            FROM directory.user_object_permisions
            WHERE user_name = @UserName
                    AND user_password = @UserPassword
            ORDER BY application_name";

            return _ApplicationList;
        }

        public static string SchemaList()
        {
            string _SchemaList =
            @"SELECT DISTINCT
                        application_schema_id,
                        schema_label
            FROM directory.user_object_permisions
            WHERE application_name = @appDbName
                AND user_name = @UserName
                AND user_password = @UserPassword
            ORDER BY schema_label";

            return _SchemaList;
        }

        public static string TableList()
        {
            string _TabelList =
            @"SELECT DISTINCT 
                        application_table_id,
                        table_label
            FROM directory.user_object_permisions
            WHERE application_name = @appDbName
                AND user_name = @UserName
                AND user_password = @UserPassword
                AND application_schema_id = @ApplicationSchemaId
            ORDER BY table_label";

            return _TabelList;
        }

        public static string TableFilterDefault()
        {
            string _TableFilterDefault =
            @"SELECT 
                    filter_definition
            FROM metadata.application_filter
            WHERE application_table_id = @sqlparam
                AND order_by = 1";

            return _TableFilterDefault;
        }

        public static string TableFilterSelected()
        {
            string _TableFilterSelected =
            @"SELECT 
                    filter_definition
            FROM metadata.application_filter
            WHERE application_filter_id = @sqlparam";

            return _TableFilterSelected;
        }

        public static string TableFilterList()
        {
            string _TableFilterList =
            @"SELECT
                    application_filter_id AS value_member,
                    filter_name AS display_member
            FROM metadata.application_filter
            WHERE application_table_id = @applicationTableId
            ORDER BY order_by";

            return _TableFilterList;
        }
        
        public static string TableMetadata()
        {
            string _TableMetadata =
            @"SELECT 
                        a.application_name,
                        t.application_table_id,
                        t.table_name,
                        t.table_label,
                        t.dml as table_dml,
                        t.order_by as table_order_by,
                        COALESCE(t.page_row_count,'25') as page_row_count,
                        s.schema_name,
                        s.schema_label,
                        t.table_Key,
                        t.application_table_id
                FROM metadata.application a
                        INNER JOIN metadata.application_schema apps ON a.application_id = apps.application_id
                        INNER JOIN metadata.application_table t ON t.application_schema_id = apps.application_schema_id
                        INNER JOIN metadata.schema s ON apps.schema_id = s.schema_id
                WHERE application_table_id = @tabId";

            return _TableMetadata;
        }

        public static string ColumnMetadata()
        {
            string _ColumnMetadata =
         @"SELECT c.column_name,
                           COALESCE(c.column_label, c.column_name) as column_label,
                           c.row_source,
                           c.filter,
                           c.order_by,
                           ct.window_control_type,
                           c.window_control_enabled,
                           c.column_default_value,
                           c.column_required_value,
                           c.column_description
                    FROM metadata.application_column c
                         INNER JOIN metadata.window_control_type ct on c.window_control_type_id = ct.window_control_type_id
                    WHERE application_table_id = @applicationtableid
                    ORDER BY c.window_layout_order";

            return _ColumnMetadata;
        }

        public static string ColumnMetadataForColumn()
        {
            string _ColumnMetadataForColumn =
         @"SELECT c.column_name,
                           COALESCE(c.column_label, c.column_name) as column_label,
                           c.row_source,
                           c.filter,
                           c.order_by,
                           ct.window_control_type,
                           c.window_control_enabled,
                           c.column_default_value,
                           c.column_required_value,
                           c.column_description
                    FROM metadata.application_column c
                         INNER JOIN metadata.window_control_type ct on c.window_control_type_id = ct.window_control_type_id
                    WHERE application_table_id = @applicationtableid
                        AND c.column_name = @colname
                    ORDER BY c.window_layout_order";

            return _ColumnMetadataForColumn;
        }
    }
}
