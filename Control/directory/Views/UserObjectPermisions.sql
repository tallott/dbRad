

CREATE VIEW [directory].[UserObjectPermisions]
AS
     SELECT u.UserName,
            u.UserPassword,
            COALESCE(a.ApplicationName, s.ApplicationName, t.ApplicationName) AS ApplicationName,
            COALESCE(a.ApplicationSchemaId, s.ApplicationSchemaId, t.ApplicationSchemaId) AS ApplicationSchemaId,
            COALESCE(a.SchemaName, s.SchemaName, t.SchemaName) AS SchemaName,
		  COALESCE(a.SchemaLabel, s.SchemaLabel, t.SchemaLabel) AS SchemaLabel,
            COALESCE(a.ApplicationTableId, s.ApplicationTableId, t.ApplicationTableId) AS ApplicationTableId,
            COALESCE(a.TableName, s.TableName, t.TableName) AS TableName,
            COALESCE(a.TableLabel, s.TableLabel, t.TableLabel) AS TableLabel
     FROM directory.[user] u
          INNER JOIN directory.UserRole ur ON u.UserId = ur.UserId
          LEFT OUTER JOIN
(
    SELECT ra.RoleId,
           a.ApplicationName,
           s.ApplicationSchemaId,
           sch.SchemaName,
		 sch.SchemaLabel,
           t.ApplicationTableId,
           t.TableName,
           t.TableLabel
    FROM directory.RoleApplication ra
         INNER JOIN metadata.[Application] a ON a.ApplicationId = ra.ApplicationId
         INNER JOIN metadata.ApplicationSchema s ON s.ApplicationId = a.ApplicationId
         INNER JOIN metadata.[Schema] sch ON sch.SchemaId = s.SchemaId
         INNER JOIN metadata.ApplicationTable t ON t.ApplicationSchemaId = s.ApplicationSchemaId
) a ON ur.RoleId = a.RoleId
          LEFT OUTER JOIN
(
    SELECT ras.RoleId,
           a.ApplicationName,
           s.ApplicationSchemaId,
           sch.SchemaName,
		 sch.SchemaLabel,
           t.ApplicationTableId,
           t.TableName,
           t.TableLabel
    FROM directory.RoleApplicationSchema ras
         INNER JOIN metadata.ApplicationSchema s ON ras.ApplicationSchemaId = s.ApplicationSchemaId
         INNER JOIN metadata.[Application] a ON a.ApplicationId = s.ApplicationId
         INNER JOIN metadata.[Schema] sch ON sch.SchemaId = s.SchemaId
         INNER JOIN metadata.ApplicationTable t ON s.ApplicationSchemaId = t.ApplicationSchemaId
) s ON ur.RoleId = s.RoleId
          LEFT OUTER JOIN
(
    SELECT rat.RoleId,
           a.ApplicationName,
           s.ApplicationSchemaId,
           sch.SchemaName,
		 sch.SchemaLabel,
           t.ApplicationTableId,
           t.TableName,
           t.TableLabel
    FROM directory.RoleApplicationTable rat
         INNER JOIN metadata.ApplicationTable t ON t.ApplicationTableId = rat.ApplicationTableId
         INNER JOIN metadata.ApplicationSchema s ON s.ApplicationSchemaId = t.ApplicationSchemaId
         INNER JOIN metadata.[Application] a ON a.ApplicationId = s.ApplicationId
         INNER JOIN metadata.[Schema] sch ON sch.SchemaId = s.SchemaId
) t ON ur.RoleId = t.RoleId;

