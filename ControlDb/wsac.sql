USE [wsac]
GO
/****** Object:  Trigger [CalcPoints]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[fish].[CalcPoints]'))
DROP TRIGGER [fish].[CalcPoints]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciesPoints_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints] DROP CONSTRAINT [FK_tblSpeciesPoints_tblSpecie]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciePoints_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints] DROP CONSTRAINT [FK_tblSpeciePoints_tblSection]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSectionPointsRule_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSectionPointsRule]'))
ALTER TABLE [fish].[tblSectionPointsRule] DROP CONSTRAINT [FK_tblSectionPointsRule_tblSection]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblPairs_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblPair]'))
ALTER TABLE [fish].[tblPair] DROP CONSTRAINT [FK_tblPairs_tblSeasons]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblMember_tblGender]') AND parent_object_id = OBJECT_ID(N'[fish].[tblMember]'))
ALTER TABLE [fish].[tblMember] DROP CONSTRAINT [FK_tblMember_tblGender]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComps_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp] DROP CONSTRAINT [FK_tblComps_tblSeasons]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComp_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp] DROP CONSTRAINT [FK_tblComp_tblSection]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCoC_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblChampion]'))
ALTER TABLE [fish].[tblChampion] DROP CONSTRAINT [FK_tblCoC_tblMembers]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] DROP CONSTRAINT [FK_tblCatch_tblSpecie]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] DROP CONSTRAINT [FK_tblCatch_tblMembers]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblComps]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] DROP CONSTRAINT [FK_tblCatch_tblComps]
GO
/****** Object:  Table [fish].[tblSpeciePoints]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]') AND type in (N'U'))
DROP TABLE [fish].[tblSpeciePoints]
GO
/****** Object:  Table [fish].[tblSpecie]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSpecie]') AND type in (N'U'))
DROP TABLE [fish].[tblSpecie]
GO
/****** Object:  Table [fish].[tblSectionPointsRule]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSectionPointsRule]') AND type in (N'U'))
DROP TABLE [fish].[tblSectionPointsRule]
GO
/****** Object:  Table [fish].[tblSection]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSection]') AND type in (N'U'))
DROP TABLE [fish].[tblSection]
GO
/****** Object:  Table [fish].[tblSeason]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSeason]') AND type in (N'U'))
DROP TABLE [fish].[tblSeason]
GO
/****** Object:  Table [fish].[tblPair]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblPair]') AND type in (N'U'))
DROP TABLE [fish].[tblPair]
GO
/****** Object:  Table [fish].[tblMember]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblMember]') AND type in (N'U'))
DROP TABLE [fish].[tblMember]
GO
/****** Object:  Table [fish].[tblGender]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblGender]') AND type in (N'U'))
DROP TABLE [fish].[tblGender]
GO
/****** Object:  Table [fish].[tblComp]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblComp]') AND type in (N'U'))
DROP TABLE [fish].[tblComp]
GO
/****** Object:  Table [fish].[tblChampion]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblChampion]') AND type in (N'U'))
DROP TABLE [fish].[tblChampion]
GO
/****** Object:  Table [fish].[tblCatch]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblCatch]') AND type in (N'U'))
DROP TABLE [fish].[tblCatch]
GO
/****** Object:  UserDefinedFunction [fish].[RareBonus]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[RareBonus]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [fish].[RareBonus]
GO
/****** Object:  UserDefinedFunction [fish].[LineBonus]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[LineBonus]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [fish].[LineBonus]
GO
/****** Object:  Schema [fish]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'fish')
DROP SCHEMA [fish]
GO
/****** Object:  Schema [fish]    Script Date: 12/08/2017 10:21:36 p.m. ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'fish')
EXEC sys.sp_executesql N'CREATE SCHEMA [fish]'

GO
/****** Object:  UserDefinedFunction [fish].[LineBonus]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[LineBonus]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [fish].[LineBonus](@CatchID INT)
RETURNS FLOAT
AS
	BEGIN
	    DECLARE @result FLOAT;

	    SELECT @result = CASE sectionId
						WHEN 1
						THEN CASE Type
							    WHEN ''S''
							    THEN CASE
									   WHEN Ratio >= 0.5
									   THEN IntPart + CASE
													  WHEN Ratio - IntPart >= 0.5
													  THEN 0.5
													  ELSE 0
												   END / 0.5 * BasicPoints
									   ELSE 0
								    END
							    WHEN ''N''
							    THEN CASE
									   WHEN Ratio >= 1.5
									   THEN((IntPart + 0.5) + CASE
															WHEN Ratio - IntPart < 0.5
															THEN-1
															ELSE 0
														 END - 0.5) * BasicPoints
									   ELSE 0
								    END
							END
						WHEN 2
						THEN FLOOR((Ratio / 0.5) - 1) * BasicPoints
					 END
	    FROM
	    (
		   SELECT sp.BasicPoints,
				cmp.SectionID,
				s.Type,
				c.weight / c.BSL AS        Ratio,
				FLOOR(c.weight / c.BSL) AS IntPart
		   FROM fish.tblCatch AS c
			   INNER JOIN fish.tblComp AS cmp ON c.CompID = cmp.CompID
			   INNER JOIN fish.tblSpecie AS s ON c.SpecieID = s.SpecieID
			   INNER JOIN fish.tblSpeciePoints AS sp ON s.SpecieID = sp.SpecieID
											    AND cmp.Start BETWEEN ValidFromDate AND isnull(ValidToDate, GETDATE())
		   WHERE c.CatchID = @CatchID
	    ) AS t1;
	    RETURN @result;
	END;' 
END

GO
/****** Object:  UserDefinedFunction [fish].[RareBonus]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[RareBonus]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [fish].[RareBonus]
(@CatchID INT = 1296
)
RETURNS INT
AS
	BEGIN
	    DECLARE @RareBonus INT;

	    SELECT @RareBonus = CASE
						   WHEN RANK() OVER(PARTITION BY c.SpecieID,
												   c.MemberID,
												   c.CompID ORDER BY CatchID) = 1
						   THEN RareBonusPoints
						   ELSE 0
					    END
	    FROM fish.tblCatch AS c
		    INNER JOIN fish.tblComp AS cmp ON c.CompID = cmp.CompID
		    INNER JOIN fish.tblSpeciePoints AS sp ON c.SpecieID = sp.SpecieID
											AND cmp.Start BETWEEN sp.ValidFromDate AND isnull(ValidToDate, GETDATE())
	    WHERE c.CatchID = @CatchID;
	    RETURN @RareBonus;
	END;' 
END

GO
/****** Object:  Table [fish].[tblCatch]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblCatch]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblCatch](
	[CatchID] [int] IDENTITY(1,1) NOT NULL,
	[SpecieID] [int] NULL,
	[MemberID] [int] NOT NULL,
	[CompID] [int] NOT NULL,
	[Weight] [float] NOT NULL,
	[BSL] [float] NULL,
	[Score] [float] NULL,
 CONSTRAINT [PK_tblCatch] PRIMARY KEY CLUSTERED 
(
	[CatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblChampion]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblChampion]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblChampion](
	[ChampionID] [int] IDENTITY(1,1) NOT NULL,
	[MemberID] [int] NULL,
 CONSTRAINT [PK_tblChampion] PRIMARY KEY CLUSTERED 
(
	[ChampionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblComp]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblComp]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblComp](
	[CompID] [int] IDENTITY(1,1) NOT NULL,
	[SectionID] [int] NOT NULL,
	[SeasonID] [int] NULL,
	[Number] [float] NULL,
	[Start] [date] NULL,
	[Finish] [date] NULL,
	[Report] [nvarchar](max) NULL,
	[Tide] [nvarchar](50) NULL,
	[Moon] [nvarchar](50) NULL,
	[Weather] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblComps] PRIMARY KEY CLUSTERED 
(
	[CompID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblGender]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblGender]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblGender](
	[GenderID] [int] IDENTITY(1,1) NOT NULL,
	[Gender] [varchar](10) NULL,
 CONSTRAINT [PK_tblGender] PRIMARY KEY CLUSTERED 
(
	[GenderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [fish].[tblMember]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblMember]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblMember](
	[MemberID] [int] IDENTITY(1,1) NOT NULL,
	[Lastname] [nvarchar](255) NULL,
	[Firstname] [nvarchar](255) NULL,
	[EOSReport] [bit] NOT NULL,
	[DateOfBirth] [date] NULL,
	[GenderID] [int] NULL,
 CONSTRAINT [PK_tblMembers] PRIMARY KEY CLUSTERED 
(
	[MemberID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblPair]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblPair]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblPair](
	[PairID] [int] IDENTITY(1,1) NOT NULL,
	[PairDescription] [nvarchar](50) NULL,
	[SeasonID] [int] NULL,
	[SectionID] [int] NULL,
	[MemberID1] [int] NULL,
	[MemberID2] [int] NULL,
 CONSTRAINT [PK_tblPairs] PRIMARY KEY CLUSTERED 
(
	[PairID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblSeason]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSeason]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblSeason](
	[SeasonID] [int] IDENTITY(1,1) NOT NULL,
	[Season] [nvarchar](50) NULL,
	[IsCurrent] [bit] NOT NULL,
 CONSTRAINT [PK_tblSeasons] PRIMARY KEY CLUSTERED 
(
	[SeasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblSection]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSection]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblSection](
	[SectionID] [int] IDENTITY(1,1) NOT NULL,
	[Section] [nvarchar](20) NULL,
 CONSTRAINT [PK_tblSection] PRIMARY KEY CLUSTERED 
(
	[SectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblSectionPointsRule]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSectionPointsRule]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblSectionPointsRule](
	[SectionPointsRuleID] [int] IDENTITY(1,1) NOT NULL,
	[SectionID] [int] NULL,
	[RuleMacro] [varchar](255) NULL,
	[ValidFromDate] [date] NULL,
	[ValidToDate] [date] NULL,
 CONSTRAINT [PK_tblSectionPointsRule] PRIMARY KEY CLUSTERED 
(
	[SectionPointsRuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [fish].[tblSpecie]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSpecie]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblSpecie](
	[SpecieID] [int] IDENTITY(1,1) NOT NULL,
	[Species] [nvarchar](255) NULL,
	[Basic] [float] NULL,
	[perkg] [float] NULL,
	[rarebonus] [float] NULL,
	[Type] [nvarchar](1) NULL,
	[BonusWeight] [float] NULL,
 CONSTRAINT [PK_tblSpecies] PRIMARY KEY CLUSTERED 
(
	[SpecieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [fish].[tblSpeciePoints]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]') AND type in (N'U'))
BEGIN
CREATE TABLE [fish].[tblSpeciePoints](
	[SpeciePointsID] [int] IDENTITY(1,1) NOT NULL,
	[SpecieID] [int] NOT NULL,
	[SectionID] [int] NOT NULL,
	[BasicPoints] [float] NOT NULL,
	[PerKgPoints] [float] NOT NULL,
	[RareBonusPoints] [float] NULL,
	[WeightBonusPoints] [float] NULL,
	[ValidFromDate] [date] NOT NULL,
	[ValidToDate] [date] NULL,
 CONSTRAINT [PK_tblSpeciesPoints] PRIMARY KEY CLUSTERED 
(
	[SpeciePointsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblComps]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch]  WITH CHECK ADD  CONSTRAINT [FK_tblCatch_tblComps] FOREIGN KEY([CompID])
REFERENCES [fish].[tblComp] ([CompID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblComps]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] CHECK CONSTRAINT [FK_tblCatch_tblComps]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch]  WITH CHECK ADD  CONSTRAINT [FK_tblCatch_tblMembers] FOREIGN KEY([MemberID])
REFERENCES [fish].[tblMember] ([MemberID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] CHECK CONSTRAINT [FK_tblCatch_tblMembers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch]  WITH CHECK ADD  CONSTRAINT [FK_tblCatch_tblSpecie] FOREIGN KEY([SpecieID])
REFERENCES [fish].[tblSpecie] ([SpecieID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCatch_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblCatch]'))
ALTER TABLE [fish].[tblCatch] CHECK CONSTRAINT [FK_tblCatch_tblSpecie]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCoC_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblChampion]'))
ALTER TABLE [fish].[tblChampion]  WITH CHECK ADD  CONSTRAINT [FK_tblCoC_tblMembers] FOREIGN KEY([MemberID])
REFERENCES [fish].[tblMember] ([MemberID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblCoC_tblMembers]') AND parent_object_id = OBJECT_ID(N'[fish].[tblChampion]'))
ALTER TABLE [fish].[tblChampion] CHECK CONSTRAINT [FK_tblCoC_tblMembers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComp_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp]  WITH CHECK ADD  CONSTRAINT [FK_tblComp_tblSection] FOREIGN KEY([SectionID])
REFERENCES [fish].[tblSection] ([SectionID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComp_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp] CHECK CONSTRAINT [FK_tblComp_tblSection]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComps_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp]  WITH CHECK ADD  CONSTRAINT [FK_tblComps_tblSeasons] FOREIGN KEY([SeasonID])
REFERENCES [fish].[tblSeason] ([SeasonID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblComps_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblComp]'))
ALTER TABLE [fish].[tblComp] CHECK CONSTRAINT [FK_tblComps_tblSeasons]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblMember_tblGender]') AND parent_object_id = OBJECT_ID(N'[fish].[tblMember]'))
ALTER TABLE [fish].[tblMember]  WITH CHECK ADD  CONSTRAINT [FK_tblMember_tblGender] FOREIGN KEY([GenderID])
REFERENCES [fish].[tblGender] ([GenderID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblMember_tblGender]') AND parent_object_id = OBJECT_ID(N'[fish].[tblMember]'))
ALTER TABLE [fish].[tblMember] CHECK CONSTRAINT [FK_tblMember_tblGender]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblPairs_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblPair]'))
ALTER TABLE [fish].[tblPair]  WITH CHECK ADD  CONSTRAINT [FK_tblPairs_tblSeasons] FOREIGN KEY([SeasonID])
REFERENCES [fish].[tblSeason] ([SeasonID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblPairs_tblSeasons]') AND parent_object_id = OBJECT_ID(N'[fish].[tblPair]'))
ALTER TABLE [fish].[tblPair] CHECK CONSTRAINT [FK_tblPairs_tblSeasons]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSectionPointsRule_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSectionPointsRule]'))
ALTER TABLE [fish].[tblSectionPointsRule]  WITH CHECK ADD  CONSTRAINT [FK_tblSectionPointsRule_tblSection] FOREIGN KEY([SectionID])
REFERENCES [fish].[tblSection] ([SectionID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSectionPointsRule_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSectionPointsRule]'))
ALTER TABLE [fish].[tblSectionPointsRule] CHECK CONSTRAINT [FK_tblSectionPointsRule_tblSection]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciePoints_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints]  WITH CHECK ADD  CONSTRAINT [FK_tblSpeciePoints_tblSection] FOREIGN KEY([SectionID])
REFERENCES [fish].[tblSection] ([SectionID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciePoints_tblSection]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints] CHECK CONSTRAINT [FK_tblSpeciePoints_tblSection]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciesPoints_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints]  WITH CHECK ADD  CONSTRAINT [FK_tblSpeciesPoints_tblSpecie] FOREIGN KEY([SpecieID])
REFERENCES [fish].[tblSpecie] ([SpecieID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[fish].[FK_tblSpeciesPoints_tblSpecie]') AND parent_object_id = OBJECT_ID(N'[fish].[tblSpeciePoints]'))
ALTER TABLE [fish].[tblSpeciePoints] CHECK CONSTRAINT [FK_tblSpeciesPoints_tblSpecie]
GO
/****** Object:  Trigger [fish].[CalcPoints]    Script Date: 12/08/2017 10:21:36 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[fish].[CalcPoints]'))
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Tim Allott
-- Create date: 
-- Description:	Calculates Catch record points
-- =============================================
CREATE TRIGGER [fish].[CalcPoints] 
   ON  [fish].[tblCatch] 
   AFTER INSERT,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here

END
' 
GO
ALTER TABLE [fish].[tblCatch] ENABLE TRIGGER [CalcPoints]
GO
