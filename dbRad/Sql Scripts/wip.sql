/****** Script for SelectTopNRows command from SSMS  ******/
SELECT concat('SELECT c.',c1.ColumnName,' AS valueMember,'''' AS displayMember FROM [',s.SchemaName,'].', t1.TableName,' as c INNER JOIN ','metadata.ApplicationTable t ON t.ApplicationTableId = c.ApplicationTableId
ORDER BY 2')
  FROM [Control].[metadata].[ApplicationTableRelation] tr
   INNER JOIN metadata.ApplicationColumn c1 ON tr.[ParentColumnID] = c1.ApplicationColumnId
     INNER JOIN metadata.ApplicationTable t1 ON t1.ApplicationTableId = c1.ApplicationTableId
	 inner join metadata.ApplicationSchema sa on sa.ApplicationSchemaId = t1.ApplicationSchemaId
	 inner join metadata.[Schema] s on s.SchemaId = sa.SchemaId
     