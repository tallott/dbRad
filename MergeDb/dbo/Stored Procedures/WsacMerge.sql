create procedure WsacMerge
as

--Gender
INSERT INTO wsac.fish.tblGender(Gender)
       SELECT t1.Gender
       FROM
       (
           SELECT 'Male' AS gender
           UNION
           SELECT 'Female'
           EXCEPT
           SELECT Gender
           FROM wsac.fish.tblGender
       ) t1; 
    
--Gender
INSERT INTO wsac.fish.tblSection(Section)
       SELECT t1.Section
       FROM
       (
           SELECT 'Shore' AS Section
           UNION
           SELECT 'Boat'
           EXCEPT
           SELECT Section
           FROM wsac.fish.tblSection
       ) t1; 

--Members
INSERT INTO wsac.fish.tblMember
(Lastname,
 Firstname,
 GenderID
)
       SELECT [LastName],
              [Firstname],
              dbo.GetGenderFromFirstName([Firstname])
       FROM [merge].[dbo].[Member]
       EXCEPT
       SELECT [LastName],
              [Firstname],
              GenderID
       FROM wsac.fish.tblMember;

--Seasons
INSERT INTO wsac.fish.tblSeason(season)
       SELECT Season
       FROM
       (
           SELECT Season
           FROM dbo.Season
           EXCEPT
           SELECT season
           FROM wsac.fish.tblSeason
       ) t1
       ORDER BY TRY_CONVERT( INT, LEFT(season, 4));

--Comps
--delete from wsac.fish.tblComp
INSERT INTO wsac.fish.tblComp
(SectionID,
 SeasonID,
 Number,
 Start,
 Finish,
 Report,
 Tide,
 Moon,
 Weather
)
       SELECT SectionID,
              SeasonID,
              Number,
              NULLIF(Start, '1 Jan 1900'),
              NULLIF(Finish, '1 Jan 1900'),
              NULLIF(Report, ''),
              NULLIF(Tide, ''),
              NULLIF(Moon, ''),
              NULLIF(Weather, '')
       FROM
       (
           SELECT CASE
                      WHEN S_CompID IS NOT NULL
                      THEN
           (
               SELECT SectionID
               FROM wsac.fish.tblSection
               WHERE section = 'Shore'
           )
                      WHEN B_CompID IS NOT NULL
                      THEN
           (
               SELECT SectionID
               FROM wsac.fish.tblSection
               WHERE section = 'Boat'
           )
                  END AS SectionID,
                  s.SeasonID,
                  Number,
                  isnull(Start, '1 Jan 1900') AS Start,
                  isnull(Finish, '1 Jan 1900') AS Finish,
                  Report,
                  isnull(Tide, '') AS Tide,
                  Moon,
                  Weather
           FROM dbo.Comp c
                INNER JOIN wsac.fish.tblSeason s ON s.Season = c.Season
           EXCEPT
           SELECT SectionID,
                  SeasonID,
                  Number,
                  isnull(Start, '1 Jan 1900') AS Start,
                  isnull(Finish, '1 Jan 1900') AS Finish,
                  Report,
                  isnull(Tide, '') AS Tide,
                  Moon,
                  Weather
           FROM wsac.fish.tblComp
       ) t1
       ORDER BY 1,
                2,
                3;

--Specie
INSERT INTO wsac.fish.tblSpecie
(Species,
 ClubName,
 Type
)
       SELECT Species,
              ClubName,
              Type
       FROM dbo.Specie
       EXCEPT
       SELECT Species,
              ClubName,
              Type
       FROM wsac.fish.tblSpecie;

--SpeciesPoints


INSERT INTO wsac.fish.tblSpeciePoints
(SpecieID,
 SectionID,
 BasicPoints,
 PerKgPoints,
 RareBonusPoints,
 WeightBonusPoints,
 ValidFromDate,
 ValidToDate
)
       SELECT SpecieID,
              SectionID,
              BasicPoints,
              PerKgPoints,
              RareBonusPoints,
              WeightBonusPoints,
              ValidFromDate,
              ValidToDate
       FROM
       (
           SELECT
           (
               SELECT specieId
               FROM wsac.fish.tblSpecie
               WHERE Species = sh.Species
           ) AS SpecieId,
           CASE
               WHEN S_SpeciesID IS NOT NULL
               THEN
           (
               SELECT SectionId
               FROM wsac.fish.tblSection
               WHERE section = 'Shore'
           )
               WHEN B_SpeciesID IS NOT NULL
               THEN
           (
               SELECT SectionId
               FROM wsac.fish.tblSection
               WHERE section = 'Boat'
           )
           END AS SectionId,
           Basic AS BasicPoints,
           perkg AS PerKgPoints,
           rarebonus AS RareBonusPoints,
           BonusWeight AS WeightBonusPoints,
           DateFrom AS ValidFromDate,
           DateTo AS ValidToDate
           FROM dbo.SpeciesHistory sh
           EXCEPT
           SELECT SpecieID,
                  SectionID,
                  BasicPoints,
                  PerKgPoints,
                  RareBonusPoints,
                  WeightBonusPoints,
                  ValidFromDate,
                  ValidToDate
           FROM wsac.fish.tblSpeciePoints
       ) t1
       ORDER BY SpecieID,
                SectionID,
                ValidFromDate;

--Catch

DELETE FROM wsac.fish.tblCatch;
INSERT INTO wsac.fish.tblCatch
(SpecieID,
 MemberID,
 CompID,
 Weight,
 BSL
)
       SELECT spc.SpecieID,
              mbr.MemberID,
              cmp.CompID,
              Weight,
              BSL
       FROM dbo.Catch c
            CROSS APPLY
       (
           SELECT SpecieId
           FROM wsac.fish.tblSpecie
           WHERE Species = c.Species
       ) AS spc
            CROSS APPLY
       (
           SELECT MemberId
           FROM wsac.fish.tblMember
           WHERE concat(LastName, '~', FirstName) = c.FullName
       ) AS mbr
            CROSS APPLY
       (
           SELECT CompId
           FROM wsac.fish.tblComp
           WHERE SectionID =
           (
               SELECT SectionID
               FROM wsac.fish.tblSection
               WHERE section = CASE
                                   WHEN c.S_CatchId IS NOT NULL
                                   THEN 'SHORE'
                                   WHEN c.B_CatchId IS NOT NULL
                                   THEN 'Boat'
                               END
           )
                 AND SeasonID =
           (
               SELECT SeasonId
               FROM wsac.fish.tblSeason
               WHERE Season = c.Season
           )
                 AND Number = c.Number
       ) cmp;

--Champion

INSERT INTO wsac.fish.tblChampion(MemberID)
       SELECT MemberID
       FROM dbo.Champion c
            INNER JOIN wsac.fish.tblMember mbr ON c.FullName = concat(mbr.Lastname, '~', mbr.Firstname)
       EXCEPT
       SELECT MemberId
       FROM wsac.fish.tblChampion;
	  
--Pairs