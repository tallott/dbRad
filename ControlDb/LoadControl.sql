USE [Control]
GO
SET IDENTITY_INSERT [dbo].[ApplicationDatabase] ON 
GO
INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (1, N'Control')
GO
INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (2, N'Test')
GO
INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (3, N'Test2')
GO
INSERT [dbo].[ApplicationDatabase] ([ApplicationDatabaseId], [DatabaseName]) VALUES (4, N'wsac')
GO
SET IDENTITY_INSERT [dbo].[ApplicationDatabase] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicationSchema] ON 
GO
INSERT [dbo].[ApplicationSchema] ([ApplicationSchemaId], [SchemaName]) VALUES (1, N'dbo')
GO
INSERT [dbo].[ApplicationSchema] ([ApplicationSchemaId], [SchemaName]) VALUES (2, N'fish')
GO
SET IDENTITY_INSERT [dbo].[ApplicationSchema] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicationTable] ON 
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (1, 2, 1, N'AnimalGroup', N'Animal Group', N'SELECT AnimalGroupId AS ID,
       AnimalGroupCode,
       AnimalGroupDesc AS Desription
FROM dbo.AnimalGroup', N'AnimalGroupId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (2, 2, 1, N'Animal', N'Animal', N'SELECT t2.AnimalId,
       t2.AnimalCode,
       t2.AnimalDesc,
       t1.AnimalGroupCode
FROM dbo.Animal t2
     INNER JOIN dbo.AnimalGroup t1 ON t1.AnimalGroupId = t2.AnimalGroupId', N'AnimalId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (3, 3, 1, N'Animal', N'Animal', N'SELECT t2.AnimalId,
       t2.AnimalCode,
       t2.AnimalDesc,
       t1.AnimalGroupCode
FROM dbo.Animal t2
     INNER JOIN dbo.AnimalGroup t1 ON t1.AnimalGroupId = t2.AnimalGroupId', N'AnimalId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (4, 1, 1, N'ApplicationTable', N'Table', N'SELECT t.ApplicationTableId,
       d.DatabaseName,
	   s.SchemaName,
       t.TableName
FROM ApplicationTable AS t 
	 INNER JOIN ApplicationDatabase AS d on t.ApplicationDatabaseId = d.ApplicationDatabaseId
	 INNER JOIN ApplicationSchema AS s on t.ApplicationSchemaId  = s.ApplicationSchemaId', N'ApplicationTableId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (5, 1, 1, N'ApplicationColumn', N'Column', N'SELECT c.ApplicationColumnId,
       t.TableName,
       c.ColumnName,
       ct.WindowControlType
FROM ApplicationColumn AS c
     INNER JOIN ApplicationTable AS t ON c.ApplicationTableId = t.ApplicationTableId
     INNER JOIN WindowControlType AS ct ON c.WindowControlTypeId = ct.WindowControlTypeId', N'ApplicationColumnId')
GO
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
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (8, 1, 1, N'ApplicationDatabase', N'ApplicationDatabase', N'SELECT ApplicationDatabaseId,
       DatabaseName
FROM ApplicationDatabase', N'ApplicationDatabaseId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (9, 1, 1, N'ApplicationSchema', N'ApplicationSchema', N'SELECT ApplicationSchemaId,
       SchemaName
FROM ApplicationSchema', N'ApplicationSchemaId')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (10, 4, 2, N'tblCatch', N'tblCatch', N'SELECT c.CatchID AS CatchId
	,m.Firstname
	,m.Lastname
	,s.Species
	,c.Weight
	,c.BSL
	,c.linebonus
	,c.rarebonus
	,c.Score
FROM fish.tblCatch AS c
INNER JOIN fish.tblSpecie AS s ON c.SpecieID = s.SpecieID
INNER JOIN fish.tblMember AS m ON c.MemberID = m.MemberID', N'CatchID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (11, 4, 2, N'tblChampion', N'tblChampion', N'SELECT coc.ChampionId
	,m.Lastname + '', '' + m.FirstName AS Member
FROM fish.tblChampion AS coc 
INNER JOIN fish.tblMember AS m ON coc.MemberID = m.MemberID', N'ChampionID')
GO
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
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (13, 4, 2, N'tblMember', N'tblMember', N'SELECT m.MemberID AS MemberId
	,m.Lastname + '', '' + m.Firstname AS Member
	,s.Section AS Section
	,m.EOSReport AS EosReport
FROM fish.tblMember AS m
INNER JOIN fish.tblSection AS s ON m.SectionID = s.SectionID', N'MemberID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (14, 4, 2, N'tblMemberPair', N'tblMemberPair', N'SELECT MemberPairID,PairID,MemberID FROM fish.tblMemberPair', N'MemberPairID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (15, 4, 2, N'tblPair', N'tblPair', N'SELECT p.PairID AS PairId
	,p.PairDescription as PairDescription
	,s.Season as Season
FROM fish.tblPair AS p
INNER JOIN fish.tblSeason AS s ON p.SeasonID = s.SeasonID', N'PairID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (16, 4, 2, N'tblSeason', N'tblSeason', N'SELECT SeasonID,Season,IsCurrent FROM fish.tblSeason', N'SeasonID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (17, 4, 2, N'tblSection', N'tblSection', N'SELECT SectionID,Section FROM fish.tblSection', N'SectionID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (18, 4, 2, N'tblSpecie', N'tblSpecie', N'SELECT SpecieID,Species,Basic,perkg,rarebonus,Type,BonusWeight FROM fish.tblSpecie', N'SpecieID')
GO
INSERT [dbo].[ApplicationTable] ([ApplicationTableId], [ApplicationDatabaseId], [ApplicationSchemaId], [TableName], [TableLabel], [Dml], [TableKey]) VALUES (19, 4, 2, N'tblGender', N'tblGender', N'SELECT GenderID,Gender FROM fish.tblGender', N'GenderID')
GO
SET IDENTITY_INSERT [dbo].[ApplicationTable] OFF
GO
SET IDENTITY_INSERT [dbo].[WindowControlType] ON 
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (1, N'ID')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (2, N'TEXT')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (3, N'TEXTBLOCK')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (4, N'NUM')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (5, N'CHK')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (6, N'DATE')
GO
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (7, N'COMBO')
GO
SET IDENTITY_INSERT [dbo].[WindowControlType] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicationColumn] ON 
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (1, 1, N'AnimalGroupCode', N'Codes', N'AnimalGroupCode', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (2, 1, N'AnimalGroupDesc', N'Description', N'AnimalGroupDesc', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (3, 1, N'AnimalGroupId', N'ID', N'AnimalGroupId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (4, 2, N'AnimalGroupId', N'Animal Group', N'SELECT AnimalGroupId as   ValueMember, AnimalGroupDesc as DisplayMember  FROM dbo.AnimalGroup', 4, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (5, 2, N'AnimalCode', NULL, N'AnimalCode', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (6, 2, N'AnimalDesc', NULL, N'AnimalDesc', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (7, 2, N'AnimalId', NULL, N'AnimalId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (9, 3, N'AnimalId', NULL, N'AnimalId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (10, 3, N'AnimalCode', N'Animal Code', N'AnimalCode', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (11, 3, N'AnimalDesc', NULL, N'AnimalDesc', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (12, 3, N'AnimalGroupId', N'Animal Group', N'SELECT AnimalGroupId AS ValueMember,
       AnimalGroupDesc AS DisplayMember
FROM dbo.AnimalGroup', 4, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (13, 4, N'ApplicationTableId', NULL, N'ApplicationTableId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (14, 4, N'ApplicationDatabaseId', NULL, N'SELECT ApplicationDatabaseId AS valueMember, DatabaseName AS displayMember FROM ApplicationDatabase', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (15, 4, N'ApplicationSchemaId', NULL, N'SELECT ApplicationSchemaId AS ValueMember
      ,SchemaName AS DisplayMember
  FROM ApplicationSchema', 7, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (16, 4, N'TableName', NULL, N'TableName', 2, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (17, 4, N'TableLabel', NULL, N'TableLabel', 2, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (18, 4, N'Dml', NULL, N'Dml', 3, 1, 6)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (19, 5, N'ApplicationColumnId', N'ApplicationColumnId', N'ApplicationColumnId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (20, 5, N'ApplicationTableId', N'ApplicationTableId', N'SELECT ApplicationTableId AS valueMember,
       TableName AS displayMember
FROM dbo.ApplicationTable', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (21, 5, N'ColumnName', NULL, N'ColumnName', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (22, 5, N'ColumnLable', NULL, N'ColumnLable', 2, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (23, 5, N'RowSource', NULL, N'RowSource', 3, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (24, 5, N'WindowControlTypeId', NULL, N'SELECT WindowControlTypeId as ValueMember,
     WindowControlType  as DisplayMember
FROM dbo.WindowControlType', 7, 1, 6)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (25, 5, N'WindowControlEnabled', NULL, N'WindowControlEnabled', 5, 1, 7)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (26, 5, N'WindowLayoutOrder', N'WindowLayoutOrder', N'WindowLayoutOrder', 2, 1, 8)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (27, 6, N'ApplicationFilterId', NULL, N'ApplicationFilterId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (28, 6, N'ApplicationTableId', NULL, N'SELECT ApplicationTableId AS valueMember,
       TableName AS displayMember
FROM dbo.ApplicationTable', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (29, 6, N'FilterDefinition', NULL, N'FilterDefinition', 2, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (30, 6, N'SortOrder', NULL, N'SortOrder', 2, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (31, 6, N'FilterName', NULL, N'FilterName', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (32, 4, N'TableKey', NULL, N'TableKey', 2, 1, 7)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (34, 8, N'ApplicationDatabaseId', N'ApplicationDatabaseId', N'ApplicationDatabaseId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (35, 8, N'DatabaseName', N'DatabaseName', N'DatabaseName', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (36, 9, N'ApplicationSchemaId', N'ApplicationSchemaId', N'ApplicationSchemaId', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (37, 9, N'SchemaName', N'SchemaName', N'SchemaName', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (38, 10, N'CatchID', N'CatchID', N'CatchID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (39, 10, N'SpecieID', N'SpecieID', N'SELECT SpecieID AS ValueMember, Species AS DisplayMember FROM fish.tblSpecie', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (40, 10, N'MemberID', N'MemberID', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (43, 10, N'CompID', N'CompID', N'SELECT CompID AS ValueMember,s.Season + ''-'' + convert(nvarchar(2),c.Number) AS DisplayMember FROM fish.tblComp c INNER JOIN fish.tblSeason s on s.SeasonID = c.SeasonID', 7, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (44, 10, N'Weight', N'Weight', N'Weight', 2, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (45, 10, N'BSL', N'BSL', N'BSL', 2, 1, 6)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (46, 10, N'Score', N'Score', N'Score', 2, 1, 7)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (47, 10, N'rarebonus', N'rarebonus', N'rarebonus', 2, 1, 8)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (48, 10, N'Basic', N'Basic', N'Basic', 2, 1, 9)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (49, 10, N'perkg', N'perkg', N'perkg', 2, 1, 10)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (50, 10, N'linebonus', N'linebonus', N'linebonus', 2, 1, 11)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (51, 11, N'ChampionID', N'ChampionID', N'ChampionID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (54, 11, N'MemberID', N'MemberID', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (55, 12, N'CompID', N'CompID', N'CompID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (57, 12, N'SeasonID', N'SeasonID', N'SELECT SeasonID AS ValueMember, Season AS DisplayMember FROM fish.tblSeason', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (58, 12, N'Number', N'Number', N'Number', 2, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (59, 12, N'Start', N'Start', N'Start', 6, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (60, 12, N'Finish', N'Finish', N'Finish', 6, 1, 6)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (61, 12, N'Report', N'Report', N'Report', 2, 1, 7)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (62, 12, N'Tide', N'Tide', N'Tide', 2, 1, 8)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (63, 12, N'Moon', N'Moon', N'Moon', 2, 1, 9)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (64, 12, N'Weather', N'Weather', N'Weather', 2, 1, 10)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (67, 13, N'MemberID', N'MemberID', N'MemberID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (68, 13, N'Lastname', N'Lastname', N'Lastname', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (69, 13, N'Firstname', N'Firstname', N'Firstname', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (70, 13, N'SectionID', N'SectionID', N'SELECT SectionID AS ValueMember, Section AS DisplayMember FROM fish.tblSection', 7, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (71, 13, N'EOSReport', N'EOSReport', N'EOSReport', 5, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (72, 14, N'MemberPairID', N'MemberPairID', N'MemberPairID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (73, 14, N'PairID', N'PairID', N'SELECT PairID AS ValueMember, PairDescription AS DisplayMember FROM fish.tblPair', 7, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (76, 14, N'MemberID', N'MemberID', N'SELECT MemberID AS ValueMember, FirstName + '' '' + Lastname AS DisplayMember FROM fish.tblMember', 7, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (77, 15, N'PairID', N'PairID', N'PairID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (78, 15, N'PairDescription', N'PairDescription', N'PairDescription', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (80, 15, N'SeasonID', N'SeasonID', N'SELECT SeasonID AS ValueMember, Season AS DisplayMember FROM fish.tblSeason', 7, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (82, 16, N'SeasonID', N'SeasonID', N'SeasonID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (83, 16, N'Season', N'Season', N'Season', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (84, 16, N'IsCurrent', N'IsCurrent', N'IsCurrent', 5, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (85, 17, N'SectionID', N'SectionID', N'SectionID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (86, 17, N'Section', N'Section', N'Section', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (87, 18, N'SpecieID', N'SpecieID', N'SpecieID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (88, 18, N'Species', N'Species', N'Species', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (89, 18, N'Basic', N'Basic', N'Basic', 2, 1, 3)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (90, 18, N'perkg', N'perkg', N'perkg', 2, 1, 4)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (91, 18, N'rarebonus', N'rarebonus', N'rarebonus', 2, 1, 5)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (92, 18, N'Type', N'Type', N'Type', 2, 1, 6)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (93, 18, N'BonusWeight', N'BonusWeight', N'BonusWeight', 2, 1, 7)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (94, 19, N'GenderID', N'GenderID', N'GenderID', 1, 0, 1)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (95, 19, N'Gender', N'Gender', N'Gender', 2, 1, 2)
GO
INSERT [dbo].[ApplicationColumn] ([ApplicationColumnId], [ApplicationTableId], [ColumnName], [ColumnLable], [RowSource], [WindowControlTypeId], [WindowControlEnabled], [WindowLayoutOrder]) VALUES (96, 12, N'SectionID', N'SectionID', N'
SELECT SectionID AS ValueMember, Section AS DisplayMember From fish.tblSection', 7, 1, 3)
GO
SET IDENTITY_INSERT [dbo].[ApplicationColumn] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicationFilter] ON 
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (1, 1, N'All Groups', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (2, 2, N'All Animals', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (3, 2, N'One Animal', N'AnimalId = 1', 2)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (4, 3, N'All Animals', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (5, 3, N'One Animal', N'AnimalId = 1', 2)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (6, 4, N'All Tables', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (7, 5, N'All Columns', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (8, 6, N'All Filters', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (9, 6, N'Filters', N't.TableName = $ApplicationTableId$', 2)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (10, 8, N'All ApplicationDatabase', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (11, 9, N'All ApplicationSchema', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (12, 5, N'Control Type', N' ct.WindowControlType = $WindowControlTypeId$', 21)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (13, 6, N'Filter Name', N'FilterName = $FilterName$', 40)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (14, 10, N'All tblCatch', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (15, 11, N'All tblChampion', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (16, 12, N'Comp Default', N'ss.SeasonID = $SeasonID$', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (17, 13, N'All tblMember', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (18, 14, N'All tblMemberPair', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (19, 15, N'All tblPair', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (20, 16, N'All tblSeason', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (21, 17, N'All tblSection', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (22, 18, N'All tblSpecie', N'1 = 1', 1)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (24, 10, N'For selected Member', N'm.MemberID = $MemberID$', 2)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (26, 10, N'memeber and comp', N'm.MemberID = $MemberID$ AND CompID = $CompID$', 3)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (27, 12, N'All Comps', N'1 = 1', 2)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (29, 10, N'Selected Comp', N'CompID = $CompID$', 4)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (30, 10, N'Species', N's.SpecieID =$SpecieID$', 5)
GO
INSERT [dbo].[ApplicationFilter] ([ApplicationFilterId], [ApplicationTableId], [FilterName], [FilterDefinition], [SortOrder]) VALUES (31, 19, N'All Genders', N'1 = 1', 1)
GO
SET IDENTITY_INSERT [dbo].[ApplicationFilter] OFF
GO
