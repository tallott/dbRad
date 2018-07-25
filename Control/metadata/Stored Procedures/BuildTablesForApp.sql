CREATE PROCEDURE [metadata].[BuildTablesForApp] @pAppId INT
AS
--
     DECLARE @dbname VARCHAR(50), @schemaid INT,@schemaname VARCHAR(50), @crlf VARCHAR(2)= concat(CHAR(13), CHAR(10)), @ddl VARCHAR(MAX);

--cursor for schema

     DECLARE c1 CURSOR
     FOR SELECT a.ApplicationName,
                s.SchemaId,
				s.SchemaName,
                concat('USE ', a.ApplicationName, @crlf, ' IF schema_id(''', s.SchemaName, ''') IS NULL', @crlf, '  exec (''CREATE SCHEMA ', s.SchemaName, ''');')
         FROM [metadata].[Schema] s
              INNER JOIN [metadata].[ApplicationSchema] sa ON s.SchemaId = sa.SchemaId
              INNER JOIN metadata.Application a ON sa.ApplicationId = a.ApplicationId
         WHERE sa.ApplicationId = @pAppId;
	--
     OPEN c1;
     FETCH NEXT FROM c1 INTO @dbname, @schemaid,@schemaname, @ddl;
     WHILE @@FETCH_STATUS = 0
         BEGIN
             PRINT(@ddl);
             EXEC (@ddl);
		--cursor for table
/*WITHIN GROUP( ORDER BY [WindowLayoutOrder])*/

             DECLARE c2 CURSOR
             FOR SELECT concat('USE ', @dbname, @crlf, ' IF object_id(''[',@schemaname,'].[', ta.TableName, ']'') IS NULL', @crlf, '  exec (''CREATE Table [',@schemaname,'].[', ta.TableName, '](', string_agg(concat(ColumnName, ' ', t.DataType), ',') WITHIN GROUP( ORDER BY [WindowLayoutOrder]), ')'');')
                 FROM Control.metadata.ApplicationTable ta
                      INNER JOIN Control.[metadata].[ApplicationSchema] sa ON ta.ApplicationSchemaId = sa.ApplicationSchemaId
                      INNER JOIN control.[metadata].[Schema] s ON s.SchemaId = sa.SchemaId
                      INNER JOIN Control.[metadata].[ApplicationColumn] ca ON ca.[ApplicationTableId] = ta.ApplicationTableId
                      CROSS APPLY
(
    SELECT CASE ca.[WindowControlTypeId]
               WHEN 1
               THEN 'INT IDENTITY(1,1) PRIMARY KEY'
               WHEN 2
               THEN 'VARCHAR(50)'
               WHEN 3
               THEN 'VARCHAR(MAX)'
               WHEN 4
               THEN 'FLOAT'
               WHEN 5
               THEN 'BIT'
               WHEN 6
               THEN 'DATE'
               WHEN 7
               THEN 'INT'
               ELSE ''
           END AS DataType
) AS t
                 WHERE sa.ApplicationId = @pAppId
                       AND s.SchemaId = @schemaid
                 GROUP BY ta.TableName;
             OPEN c2;
             FETCH NEXT FROM c2 INTO @ddl;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     PRINT(@ddl);
                     EXEC (@ddl);
                     FETCH NEXT FROM c2 INTO @ddl;
                 END;
             CLOSE c2;
             DEALLOCATE c2;
             FETCH NEXT FROM c1 INTO @dbname, @schemaid,@schemaname, @ddl;
         END;
     CLOSE c1;
     DEALLOCATE c1;
