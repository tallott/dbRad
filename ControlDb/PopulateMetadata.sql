USE wsac;

--tables
INSERT INTO Control.dbo.ApplicationTable
(ApplicationDatabaseId,
 ApplicationSchemaId,
 TableName,
 TableLabel,
 Dml,
 TableKey
)
       SELECT 1,
              1,
              t.table_name,
              t.TABLE_NAME,
              CONCAT('select * from ', T.TABLE_NAME),
              id.COLUMN_NAME
       FROM INFORMATION_SCHEMA.TABLES t
            CROSS APPLY
       (
           SELECT column_name
           FROM INFORMATION_SCHEMA.COLUMNS
           WHERE table_name = t.TABLE_NAME
                 AND ORDINAL_POSITION = 1
       ) id
            LEFT OUTER JOIN Control.[dbo].[ApplicationTable] mt ON t.table_name = mt.[TableName]
       WHERE  mt.ApplicationDatabaseId IS NULL and left(t.TABLE_NAME,3 ) = 'tbl';
		  
--filters
INSERT INTO Control.dbo.ApplicationFilter
(ApplicationTableId,
 FilterName,
 FilterDefinition,
 SortOrder
)
       SELECT mt.ApplicationTableId,
              'All '+mt.TableName,
              '1 = 1',
              1
       FROM Control.dbo.ApplicationTable mt
            LEFT OUTER JOIN Control.dbo.ApplicationFilter mf ON mt.ApplicationTableId = mf.ApplicationTableId
       WHERE mf.ApplicationFilterId IS NULL;


--columns
INSERT INTO Control.[dbo].[ApplicationColumn]
(ApplicationTableId,
 ColumnName,
 ColumnLable,
 RowSource,
 WindowControlTypeId,
 WindowControlEnabled,
 WindowLayoutOrder
)
       SELECT mt.ApplicationTableId,
              c.column_name AS ColumnName,
              c.column_name AS ColumnLable,
              c.column_name AS Rowsource,
              CASE
                  WHEN c.ORDINAL_POSITION = 1
                  THEN 1
                  WHEN fk.COLUMN_NAME IS NOT NULL
                  THEN 7
                  WHEN c.DATA_TYPE = 'BIT'
                  THEN 5
                  ELSE 2
              END AS WindowControlTypeId,
              CASE
                  WHEN c.ORDINAL_POSITION > 1
                  THEN 1
                  ELSE 0
              END,
              c.ORDINAL_POSITION
       FROM INFORMATION_SCHEMA.COLUMNS c
            INNER JOIN Control.dbo.ApplicationTable mt ON c.TABLE_NAME = mt.tablename
            LEFT OUTER JOIN
       (
           SELECT CU.COLUMN_NAME
           FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE cu
                INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS c ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
       ) fk ON c.COLUMN_NAME = fk.COLUMN_NAME
            LEFT OUTER JOIN
       (
           SELECT ColumnName,
                  TableName
           FROM Control.dbo.ApplicationColumn mc
                INNER JOIN Control.dbo.ApplicationTable mt ON mt.ApplicationTableId = mc.ApplicationTableId
       ) md ON c.COLUMN_NAME = md.ColumnName
               AND c.TABLE_NAME = md.TableName
       WHERE md.ColumnName IS NULL
	  and c.TABLE_SCHEMA = 'fish'
       ORDER BY c.TABLE_NAME,
                c.ORDINAL_POSITION;

