USE wsac;
GO

--Blow data away
DECLARE @sql NVARCHAR(MAX)= N'';
SELECT @sql+=N'
ALTER TABLE '+QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))+'.'+QUOTENAME(OBJECT_NAME(parent_object_id))+' DROP CONSTRAINT '+QUOTENAME(name)+';'
FROM sys.foreign_keys;
SELECT @sql+=N'
TRUNCATE TABLE '+QUOTENAME(OBJECT_SCHEMA_NAME(object_id))+'.'+QUOTENAME(OBJECT_NAME(object_id))+';'
FROM sys.tables;
EXEC sp_executesql
     @sql;
DISABLE TRIGGER fish.CalcPoints ON fish.tblCatch;
GO
USE [merge];
GO
EXEC [dbo].[updates];
EXEC [dbo].[WsacMerge];
USE wsac;
GO
ENABLE TRIGGER fish.CalcPoints ON fish.tblCatch;  