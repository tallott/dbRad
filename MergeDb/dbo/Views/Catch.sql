
CREATE view [dbo].[Catch] as

SELECT CatchID AS S_CatchID,
       NULL AS B_CatchId,
       spc.Species,
       concat(mbr.Lastname, '~', mbr.Firstname) AS FullName,
       ssn.Season,
       cmp.Number,
       ctch.Weight,
       ctch.BSL
FROM shore.dbo.tblCatch ctch
     INNER JOIN shore.dbo.tblSpecies spc ON spc.SpeciesID = ctch.SpeciesID
     INNER JOIN shore.dbo.tblMembers mbr ON mbr.MemberID = ctch.MemberID
     INNER JOIN shore.dbo.tblComps cmp ON cmp.CompID = ctch.CompID
     INNER JOIN shore.dbo.tblSeasons ssn ON ssn.SeasonID = cmp.SeasonID
UNION
SELECT NULL AS S_CatchID,
       ctch.CatchID AS B_Catch_Id,
       spc.Species,
       concat(mbr.Lastname, '~', mbr.Firstname) AS FullName,
       ssn.Season,
       cmp.Number,
       ctch.Weight,
       ctch.BSL
FROM boat.dbo.tblCatch ctch
     INNER JOIN boat.dbo.tblSpecies spc ON spc.SpeciesID = ctch.SpeciesID
     INNER JOIN boat.dbo.tblMembers mbr ON mbr.MemberID = ctch.MemberID
     INNER JOIN boat.dbo.tblComps cmp ON cmp.CompID = ctch.CompID
     INNER JOIN boat.dbo.tblSeasons ssn ON ssn.SeasonID = cmp.SeasonID;