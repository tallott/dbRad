CREATE VIEW [dbo].[SpeciesHistory]
AS
     SELECT S_SpeciesID,
            B_SpeciesID,
            Species,
            rarebonus,
            Basic,
            perkg,
            BonusWeight,
            grp,
            DateFrom,
            DateTo
     FROM ShoreSpecies
     UNION
     SELECT S_SpeciesID,
            B_SpeciesID,
            Species,
            rarebonus,
            Basic,
            perkg,
            BonusWeight,
            grp,
            DateFrom,
            DateTo
     FROM dbo.BoatSpecies;