USE Control;
GO
DROP TABLE IF EXISTS metadata.ObjectText;
CREATE TABLE metadata.ObjectText
(ObjectTextId      INT IDENTITY(1, 1) PRIMARY KEY,
 WindowControlType VARCHAR(50),
 ObjectText        VARCHAR(MAX)
);
INSERT INTO metadata.ObjectText
(WindowControlType,
 ObjectText
)
       SELECT DISTINCT
              wct.WindowControlType,
              rowsource AS ObjectText
       FROM control.metadata.ApplicationColumn ac
            INNER JOIN control.metadata.WindowControlType wct ON wct.WindowControlTypeId = ac.WindowControlTypeId
       WHERE wct.WindowControlType IN('COMBO', 'FILTER', 'ORDERBY')
            AND NULLIF(RowSource, '') IS NOT NULL
       UNION
       SELECT DISTINCT
              'FILTER',
              FilterDefinition
       FROM metadata.ApplicationFilter
       UNION
       SELECT DISTINCT
              'ROWSOURCE',
              DML
       FROM metadata.ApplicationTable;

	   
            

--

DROP TABLE IF EXISTS metadata.WindowObjectText;
CREATE TABLE metadata.WindowObjectText
(WindowObjectTextId  INT IDENTITY(1, 1) PRIMARY KEY,
 ApplicationColumnId INT,
 ObjectTextName      VARCHAR(50),
 ApplicationWindowId VARCHAR(50),
 ObjectTextId        INT,
 WindowControlType   VARCHAR(50)
);
INSERT INTO metadata.WindowObjectText
(ApplicationColumnId,
 ObjectTextName,
 ApplicationWindowId,
 ObjectTextId,
 WindowControlType
)
       SELECT ac.ApplicationColumnId,
              ac.ColumnName,
              ac.ApplicationTableId,
              ot.ObjectTextId,
              ot.WindowControlType
       FROM metadata.ApplicationColumn ac
            INNER JOIN [metadata].[ObjectText] ot ON ot.ObjectText = ac.RowSource
       UNION
       SELECT NULL,
              af.FilterName,
              af.ApplicationTableId,
              ot.ObjectTextId,
              ot.WindowControlType
       FROM metadata.ApplicationFilter af
            INNER JOIN [metadata].[ObjectText] ot ON ot.ObjectText = af.FilterDefinition
       UNION
       SELECT NULL,
              t.TableName,
              t.ApplicationTableId,
              ot.ObjectTextId,
              ot.WindowControlType
       FROM metadata.ApplicationTable t
            INNER JOIN [metadata].[ObjectText] ot ON ot.ObjectText = t.Dml;
SELECT *
FROM metadata.WindowObjectText;






--UPDATE ac
--  SET
--      ac.RowSourceId = WindowObjectTextId
--FROM metadata.ApplicationColumn ac
--     INNER JOIN [metadata].[WindowObjectText] wot ON wot.ObjectText = ac.RowSource
--WHERE [ObjectType] = 'ComboSql'; 

--SELECT *
--FROM metadata.ApplicationColumn; 

