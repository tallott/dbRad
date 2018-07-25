CREATE VIEW [dbo].[PkFk]
AS
     SELECT FKname.name AS FKname,
            PKtbl.name AS PKtblName,
            PKcol.name AS PKcolName,
            FKtbl.name AS FKtblName,
            FKCol.name AS FKcolName
     FROM
(
    SELECT name,
           object_id
    FROM sys.objects
) AS FKname
INNER JOIN
(
    SELECT constraint_object_id,
           constraint_column_id,
           parent_object_id,
           parent_column_id,
           referenced_object_id,
           referenced_column_id
    FROM sys.foreign_key_columns
) AS FK ON FKname.object_id = FK.constraint_object_id
INNER JOIN
(
    SELECT concat(SCHEMA_NAME(schema_id), '.', OBJECT_NAME(object_id)) AS name,
           object_id
    FROM sys.objects AS objects_2
) AS FKtbl ON FK.parent_object_id = FKtbl.object_id
INNER JOIN
(
    SELECT concat(SCHEMA_NAME(schema_id), '.', OBJECT_NAME(object_id)) AS name,
           object_id
    FROM sys.objects AS objects_1
) AS PKtbl ON FK.referenced_object_id = PKtbl.object_id
INNER JOIN
(
    SELECT object_id,
           name,
           column_id
    FROM sys.all_columns
) AS FKCol ON FK.parent_column_id = FKCol.column_id
              AND FK.parent_object_id = FKCol.object_id
INNER JOIN
(
    SELECT object_id,
           name,
           column_id
    FROM sys.all_columns AS all_columns_1
) AS PKcol ON FK.referenced_object_id = PKcol.object_id
              AND FK.referenced_column_id = PKcol.column_id;