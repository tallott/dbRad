CREATE VIEW [dbo].[Specie]
AS
     SELECT Species,
            Type,
            ClubName,
            S_SpeciesID,
            B_SpeciesID
     FROM
     (
         SELECT COALESCE(s.species, b.species) AS Species,
                b.ClubName,
                s.Type,
                s.SpeciesID AS S_SpeciesID,
                b.SpeciesID AS B_SpeciesID
         FROM shore.dbo.tblSpecies s
              FULL OUTER JOIN boat.dbo.tblSpecies b ON s.Species = b.Species
     ) t1;