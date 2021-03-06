

INSERT INTO control.metadata.ApplicationTableRelation
(ParentColumnID,
 ChildColumnId
)
       SELECT pktab.ApplicationColumnId,
              fktab.ApplicationColumnId
       FROM control.[dbo].[PkFk] fkpk
            CROSS APPLY
(
    SELECT tab.ApplicationTableId,
           col.ApplicationColumnId
    FROM control.metadata.[Schema] sch
         INNER JOIN control.metadata.ApplicationSchema appsch ON sch.SchemaId = appsch.SchemaId
         INNER JOIN control.metadata.ApplicationTable tab ON appsch.ApplicationSchemaId = tab.ApplicationSchemaId
         INNER JOIN control.metadata.ApplicationColumn col ON tab.ApplicationTableId = col.ApplicationTableId
    WHERE concat(sch.SchemaName, '.', tab.TableName) = fkpk.[PKtblName]
          AND col.ColumnName = fkpk.[PKcolName]
) AS pktab
            CROSS APPLY
(
    SELECT tab.ApplicationTableId,
           col.ApplicationColumnId
    FROM control.metadata.[Schema] sch
         INNER JOIN control.metadata.ApplicationSchema appsch ON sch.SchemaId = appsch.SchemaId
         INNER JOIN control.metadata.ApplicationTable tab ON appsch.ApplicationSchemaId = tab.ApplicationSchemaId
         INNER JOIN control.metadata.ApplicationColumn col ON tab.ApplicationTableId = col.ApplicationTableId
    WHERE concat(sch.SchemaName, '.', tab.TableName) = fkpk.[FKtblName]
          AND col.ColumnName = fkpk.[FKcolName]
) AS fktab
       EXCEPT
       SELECT ParentColumnID,
              ChildColumnId
       FROM control.metadata.ApplicationTableRelation;