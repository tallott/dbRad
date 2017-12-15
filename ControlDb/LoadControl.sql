USE [Control]
GO
SET IDENTITY_INSERT [dbo].[ApplicationDatabase] ON 

INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (1, N'Control')
INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (4, N'wsac')
SET IDENTITY_INSERT [dbo].[ApplicationDatabase] OFF
SET IDENTITY_INSERT [dbo].[ApplicationSchema] ON 

INSERT [dbo].[ApplicationSchema] ([ApplicationSchemaId], [SchemaName]) VALUES (1, N'dbo')
INSERT [dbo].[ApplicationSchema] ([ApplicationSchemaId], [SchemaName]) VALUES (2, N'fish')
SET IDENTITY_INSERT [dbo].[ApplicationSchema] OFF
SET IDENTITY_INSERT [dbo].[ApplicationTable] ON 

INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (4, 1, 1, N'ApplicationTable', N'Table', N'SELECT t.ApplicationTableId,
       d.DatabaseName,
       s.SchemaName,
       t.TableName
FROM ApplicationTable AS t
     INNER JOIN ApplicationDatabase AS d ON t.ApplicationDatabaseId = d.ApplicationDatabaseId
     INNER JOIN ApplicationSchema AS s ON t.ApplicationSchemaId = s.ApplicationSchemaId', N'ApplicationTableId')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (5, 1, 1, N'ApplicationColumn', N'Column', N'SELECT c.ApplicationColumnId,
       t.TableName,
       c.ColumnName,
       c.ColumnLable,
       ct.WindowControlType
FROM ApplicationColumn AS c
     INNER JOIN ApplicationTable AS t ON c.ApplicationTableId = t.ApplicationTableId
     INNER JOIN WindowControlType AS ct ON c.WindowControlTypeId = ct.WindowControlTypeId', N'ApplicationColumnId')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (6, 1, 1, N'ApplicationFilter', N'Filters', N'SELECT f.ApplicationFilterId,
       d.DatabaseName,
	   s.SchemaName,
       t.TableName,
       f.FilterName,
       f.FilterDefinition,
       f.SortOrder
FROM dbo.ApplicationFilter AS f
     INNER JOIN dbo.ApplicationTable AS t ON f.ApplicationTableId = t.ApplicationTableId
	 INNER JOIN ApplicationDatabase AS d on t.ApplicationDatabaseId = d.ApplicationDatabaseId
	 INNER JOIN ApplicationSchema AS s on t.ApplicationSchemaId  = s.ApplicationSchemaId', N'ApplicationFilterId')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (8, 1, 1, N'ApplicationDatabase', N'ApplicationDatabase', N'SELECT ApplicationDatabaseId,
       DatabaseName
FROM ApplicationDatabase', N'ApplicationDatabaseId')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (9, 1, 1, N'ApplicationSchema', N'ApplicationSchema', N'SELECT ApplicationSchemaId,
       SchemaName
FROM ApplicationSchema', N'ApplicationSchemaId')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (10, 4, 2, N'tblCatch', N'tblCatch', N'SELECT c.CatchID AS CatchId,
       m.Firstname,
       m.Lastname,
       s.Species,
       c.Weight,
       c.BSL,
       c.Score,
       sctn.Section
FROM fish.tblCatch AS c
     INNER JOIN fish.tblSpecie AS s ON c.SpecieID = s.SpecieID
     INNER JOIN fish.tblMember AS m ON c.MemberID = m.MemberID
     INNER JOIN fish.tblComp cmp ON cmp.CompID = c.CompID
     INNER JOIN fish.tblSection sctn ON sctn.SectionID = cmp.SectionID', N'CatchID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (11, 4, 2, N'tblChampion', N'tblChampion', N'SELECT coc.ChampionId
	,m.Lastname + '', '' + m.FirstName AS Member
FROM fish.tblChampion AS coc 
INNER JOIN fish.tblMember AS m ON coc.MemberID = m.MemberID', N'ChampionID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (12, 4, 2, N'tblComp', N'tblComp', N'SELECT
    CompID, 
    s.Section, 
    ss.Season, 
    Number as Comp, 
    CONVERT(varchar(10), Start) AS StartDate, 
    CONVERT(varchar(10), Finish) AS FinishDate
FROM            fish.tblComp c 
INNER JOIN fish.tblSection s on c.SectionID = s.SectionID
INNER JOIN fish.tblSeason ss on c.SeasonID = ss.SeasonID', N'CompID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (13, 4, 2, N'tblMember', N'tblMember', N'SELECT m.MemberID AS MemberId,
       concat(m.Lastname, char(44),char(32), m.Firstname) AS Member,
       g.Gender AS Gender
FROM fish.tblMember AS m
     INNER JOIN fish.tblGender AS g ON m.GenderID = g.GenderID', N'MemberID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (15, 4, 2, N'tblPair', N'tblPair', N'SELECT p.PairID AS PairId
	,p.PairDescription as PairDescription
	,s.Season as Season
FROM fish.tblPair AS p
INNER JOIN fish.tblSeason AS s ON p.SeasonID = s.SeasonID', N'PairID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (16, 4, 2, N'tblSeason', N'tblSeason', N'SELECT SeasonID,Season,IsCurrent FROM fish.tblSeason', N'SeasonID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (17, 4, 2, N'tblSection', N'tblSection', N'SELECT sctn.SectionID,
       sctn.Section
FROM fish.tblSection sctn', N'SectionID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (18, 4, 2, N'tblSpecie', N'tblSpecie', N'SELECT s.SpecieID,
       s.Species,
       s.ClubName,
       s.Type
FROM fish.tblSpecie AS s', N'SpecieID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (19, 4, 2, N'tblGender', N'tblGender', N'SELECT GenderID,Gender FROM fish.tblGender', N'GenderID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (20, 4, 2, N'tblSpeciePoints', N'tblSpeciePoints', N'SELECT SpeciePointsID,
       s.Species,
       sctn.Section,
       BasicPoints,
       PerKgPoints,
       RareBonusPoints,
       WeightBonusPoints,
       CONVERT(varchar(10), ValidFromDate) as VailidFromDate,
       CONVERT(varchar(10), ValidToDate) as ValidToDate
FROM fish.tblSpeciePoints sp
     INNER JOIN fish.tblSpecie s ON sp.SpecieID = s.SpecieID
     INNER JOIN fish.tblSection sctn ON sp.SectionID = sctn.SectionID', N'SpeciePointsID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (21, 4, 2, N'tblSectionPointsRule', N'tblSectionPointsRule', N'SELECT sr.SectionPointsRuleID,
   sr.RuleMacro,
   s.Section
FROM fish.tblSectionPointsRule AS sr
 INNER JOIN fish.tblSection AS s ON s.SectionID = sr.SectionID', N'SectionPointsRuleID')
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (22, 4, 2, N'tblLineClass', N'tblLineClass', N'SELECT LineClassId,
       LineClass,
       LineClassName
FROM fish.tblLineClass', N'LineClassId')
SET IDENTITY_INSERT [dbo].[ApplicationTable] OFF
SET IDENTITY_INSERT [dbo].[WindowControlType] ON 

INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (1, N'ID')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (2, N'TEXT')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (3, N'TEXTBLOCK')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (4, N'NUM')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (5, N'CHK')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (6, N'DATE')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (7, N'COMBO')
SET IDENTITY_INSERT [dbo].[WindowControlType] OFF
SET IDENTITY_INSERT [dbo].[ApplicationColumn] ON 

INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (13, 4, N'ApplicationTableId', N'ApplicationTableId', N'ApplicationTableId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (14, 4, N'ApplicationDatabaseId', N'ApplicationDatabaseId', N'SELECT ApplicationDatabaseId AS valueMember, DatabaseName AS displayMember FROM ApplicationDatabase', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (15, 4, N'ApplicationSchemaId', N'ApplicationSchemaId', N'SELECT ApplicationSchemaId AS ValueMember
      ,SchemaName AS DisplayMember
  FROM ApplicationSchema', 7, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (16, 4, N'TableName', N'TableName', N'TableName', 2, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (17, 4, N'TableLabel', N'TableLabel', N'TableLabel', 2, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (18, 4, N'Dml', N'Dml', N'Dml', 3, 1, 6)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (19, 5, N'ApplicationColumnId', N'ApplicationColumnId', N'ApplicationColumnId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (20, 5, N'ApplicationTableId', N'ApplicationTableId', N'SELECT ApplicationTableId AS valueMember,
       TableName AS displayMember
FROM dbo.ApplicationTable', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (21, 5, N'ColumnName', N'ColumnName', N'ColumnName', 2, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (22, 5, N'ColumnLable', N'ColumnLable', N'ColumnLable', 2, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (23, 5, N'RowSource', N'RowSource', N'RowSource', 3, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (24, 5, N'WindowControlTypeId', N'WindowControlTypeId', N'SELECT WindowControlTypeId as ValueMember,
     WindowControlType  as DisplayMember
FROM dbo.WindowControlType', 7, 1, 6)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (25, 5, N'WindowControlEnabled', N'WindowControlEnabled', N'WindowControlEnabled', 5, 1, 7)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (26, 5, N'WindowLayoutOrder', N'WindowLayoutOrder', N'WindowLayoutOrder', 2, 1, 8)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (27, 6, N'ApplicationFilterId', N'ApplicationFilterId', N'ApplicationFilterId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (28, 6, N'ApplicationTableId', N'ApplicationTableId', N'SELECT ApplicationTableId AS valueMember,
       TableName AS displayMember
FROM dbo.ApplicationTable', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (29, 6, N'FilterDefinition', N'FilterDefinition', N'FilterDefinition', 3, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (30, 6, N'SortOrder', N'SortOrder', N'SortOrder', 2, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (31, 6, N'FilterName', N'FilterName', N'FilterName', 2, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (32, 4, N'TableKey', N'TableKey', N'TableKey', 2, 1, 7)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (34, 8, N'ApplicationDatabaseId', N'ApplicationDatabaseIdxxxxx', N'ApplicationDatabaseId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (35, 8, N'DatabaseName', N'DatabaseName', N'DatabaseName', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (36, 9, N'ApplicationSchemaId', N'ApplicationSchemaId', N'ApplicationSchemaId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (37, 9, N'SchemaName', N'SchemaNamec', N'SchemaName', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (38, 10, N'CatchID', N'CatchID', N'CatchID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (39, 10, N'SpecieID', N'SpecieID', N'SELECT SpecieID AS ValueMember, Species AS DisplayMember FROM fish.tblSpecie', 7, 1, 40)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (40, 10, N'MemberID', N'MemberID', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 30)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (43, 10, N'CompID', N'CompID', N'SELECT cmp.CompID AS ValueMember,
       concat(sctn.Section, CHAR(32), CHAR(45), CHAR(32), ssn.Season, CHAR(32), CHAR(45), CHAR(32), cmp.Number) AS DisplayMember
FROM fish.tblComp cmp
     INNER JOIN fish.tblSeason ssn ON ssn.SeasonID = cmp.SeasonID
     INNER JOIN fish.tblSection sctn ON sctn.SectionID = cmp.SectionID
ORDER BY cmp.Number,
         ssn.SeasonID DESC,
         sctn.Section', 7, 1, 20)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (44, 10, N'Weight', N'Weight', N'Weight', 2, 1, 50)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (45, 10, N'BSL', N'BSL', N'BSL', 2, 1, 60)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (46, 10, N'Score', N'Score', N'Score', 2, 0, 70)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (51, 11, N'ChampionID', N'ChampionID', N'ChampionID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (54, 11, N'MemberID', N'MemberID', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (55, 12, N'CompID', N'CompID', N'CompID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (57, 12, N'SeasonID', N'SeasonID', N'SELECT SeasonID AS ValueMember, Season AS DisplayMember FROM fish.tblSeason', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (58, 12, N'Number', N'Number', N'Number', 2, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (59, 12, N'Start', N'Start', N'Start', 6, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (60, 12, N'Finish', N'Finish', N'Finish', 6, 1, 6)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (61, 12, N'Report', N'Report', N'Report', 2, 1, 7)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (62, 12, N'Tide', N'Tide', N'Tide', 3, 1, 8)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (63, 12, N'Moon', N'Moon', N'Moon', 2, 1, 9)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (64, 12, N'Weather', N'Weather', N'Weather', 2, 1, 10)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (67, 13, N'MemberID', N'MemberID', N'MemberID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (68, 13, N'Lastname', N'Lastname', N'Lastname', 2, 1, 20)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (69, 13, N'Firstname', N'Firstname', N'Firstname', 2, 1, 30)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (70, 13, N'GenderID', N'GenderID', N'SELECT GenderID AS ValueMember, Gender AS DisplayMember FROM fish.tblGender', 7, 1, 10)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (77, 15, N'PairID', N'PairID', N'PairID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (78, 15, N'PairDescription', N'PairDescription', N'PairDescription', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (80, 15, N'SeasonID', N'SeasonID', N'SELECT SeasonID AS ValueMember, Season AS DisplayMember FROM fish.tblSeason', 7, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (82, 16, N'SeasonID', N'SeasonID', N'SeasonID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (83, 16, N'Season', N'Season', N'Season', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (84, 16, N'IsCurrent', N'IsCurrent', N'IsCurrent', 5, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (85, 17, N'SectionID', N'SectionID', N'SectionID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (86, 17, N'Section', N'Section', N'Section', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (87, 18, N'SpecieID', N'SpecieID', N'SpecieID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (88, 18, N'Species', N'Species', N'Species', 2, 1, 20)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (92, 18, N'Type', N'Type', N'Type', 2, 1, 40)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (93, 18, N'ClubName', N'Club Name', N'ClubName', 2, 1, 30)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (94, 19, N'GenderID', N'GenderID', N'GenderID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (95, 19, N'Gender', N'Gender', N'Gender', 2, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (96, 12, N'SectionID', N'SectionID', N'SELECT SectionID AS ValueMember, Section AS DisplayMember From fish.tblSection', 7, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (97, 13, N'DateOfBirth', N'DateOfBirth', N'DateOfBirth', 6, 1, 50)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (99, 15, N'MemberID1', N'MemberID1', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (100, 15, N'MemberID2', N'MemberID2', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (101, 15, N'SectionID', N'SectionID', N'
SELECT SectionID AS ValueMember, Section AS DisplayMember From fish.tblSection', 7, 1, 10)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (105, 20, N'SpeciePointsID', N'SpeciePointsID', N'SpeciePointsID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (106, 20, N'SpecieID', N'SpecieID', N'SELECT SpecieID AS ValueMember, Species AS DisplayMember FROM fish.tblSpecie', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (107, 20, N'SectionID', N'SectionID', N'
SELECT SectionID AS ValueMember, Section AS DisplayMember From fish.tblSection', 7, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (108, 20, N'BasicPoints', N'BasicPoints', N'BasicPoints', 2, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (109, 20, N'PerKgPoints', N'PerKgPoints', N'PerKgPoints', 2, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (110, 20, N'RareBonusPoints', N'RareBonusPoints', N'RareBonusPoints', 2, 1, 6)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (111, 20, N'WeightBonusPoints', N'WeightBonusPoints', N'WeightBonusPoints', 2, 1, 7)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (112, 20, N'ValidFromDate', N'ValidFromDate', N'ValidFromDate', 6, 1, 8)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (113, 20, N'ValidToDate', N'ValidToDate', N'ValidToDate', 6, 1, 9)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (114, 21, N'SectionPointsRuleID', N'SectionPointsRuleID', N'SectionPointsRuleID', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (115, 21, N'SectionID', N'SectionID', N'SELECT SectionID AS ValueMember, Section AS DisplayMember From fish.tblSection', 7, 1, 2)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (116, 21, N'RuleMacro', N'RuleMacro', N'RuleMacro', 3, 1, 3)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (117, 21, N'ValidFromDate', N'ValidFromDate', N'ValidFromDate', 6, 1, 4)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (118, 21, N'ValidToDate', N'ValidToDate', N'ValidToDate', 6, 1, 5)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (119, 22, N'LineClassId', N'LineClassId', N'LineClassId', 1, 0, 1)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (120, 22, N'LineClass', N'LineClass', N'LineClass', 4, 1, 10)
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (121, 22, N'LineClassName', N'LineClassName', N'LineClassName', 2, 1, 20)
SET IDENTITY_INSERT [dbo].[ApplicationColumn] OFF
SET IDENTITY_INSERT [dbo].[ApplicationFilter] ON 

INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (6, 4, N'All Tables', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (7, 5, N'All Columns', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (8, 6, N'All Filters', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (9, 6, N'Filters', N't.ApplicationTableId= $ApplicationTableId$', 2)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (10, 8, N'All ApplicationDatabase', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (11, 9, N'All ApplicationSchema', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (12, 5, N'Control Type', N'ct.WindowControlTypeID = $WindowControlTypeId$', 3)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (13, 6, N'Filter Name', N'FilterName = $FilterName$', 40)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (14, 10, N'All tblCatch', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (15, 11, N'All tblChampion', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (16, 12, N'Comp Default', N'ss.SeasonID = $SeasonID$', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (17, 13, N'All tblMember', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (19, 15, N'All tblPair', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (20, 16, N'All tblSeason', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (21, 17, N'All tblSection', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (22, 18, N'All tblSpecie', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (24, 10, N'For selected Member', N'm.MemberID = $MemberID$', 2)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (26, 10, N'memeber and comp', N'm.MemberID = $MemberID$ AND cmp.CompID = $CompID$', 3)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (27, 12, N'All Comps', N'1 = 1', 2)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (29, 10, N'Selected Comp', N'CompID = $CompID$', 4)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (30, 10, N'Species', N's.SpecieID =$SpecieID$', 5)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (31, 19, N'All Genders', N'1 = 1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (32, 5, N'For tab', N't.ApplicationTableId = $ApplicationTableId$', 2)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (33, 20, N'All Rows', N'1=1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (34, 21, N'Default', N'1=1', 1)
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (35, 22, N'Default', N'1=1', 1)
SET IDENTITY_INSERT [dbo].[ApplicationFilter] OFF
