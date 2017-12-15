CREATE VIEW ShoreSpecies
AS
     SELECT t3.SpeciesID AS S_SpeciesID,
            NULL AS B_SpeciesID,
            t3.Species,
            t3.rarebonus,
            t3.Basic,
            t3.perkg,
            t3.BonusWeight,
            t3.grp,
            CONVERT(DATE, '1 Jul '+LEFT(MIN(t3.Season), CHARINDEX('/', MIN(t3.Season))-1)) AS DateFrom,
            DATEADD(d, -1, LEAD(CONVERT(DATE, '1 Jul '+LEFT(MIN(t3.Season), CHARINDEX('/', MIN(t3.Season))-1))) OVER(PARTITION BY t3.SpeciesID ORDER BY CONVERT(DATE, '1 Jul '+LEFT(MIN(t3.Season), CHARINDEX('/', MIN(t3.Season))-1)))) AS DateTo
     FROM
     (
         SELECT t2.SpeciesID,
                t2.Species,
                t2.rarebonus,
                t2.Basic,
                t2.perkg,
                t2.BonusWeight,
                t2.Season,
                SUM(t2.x) OVER(PARTITION BY t2.SpeciesId ORDER BY t2.Season) AS grp
         FROM
         (
             SELECT t1.SpeciesID,
                    t1.Species,
                    t1.rarebonus,
                    t1.Basic,
                    t1.perkg,
                    t1.BonusWeight,
                    t1.Season,
                    CASE
                        WHEN grp <> LAG(t1.grp) OVER(PARTITION BY t1.speciesId ORDER BY t1.Season)
                        THEN 1
                        ELSE 0
                    END AS x
             FROM
             (
                 SELECT ctch.SpeciesID,
                        spc.Species,
                        MAX(ctch.rarebonus) rarebonus,
                        ctch.Basic,
                        ctch.perkg,
                        spc.BonusWeight,
                        Season,
                        CONCAT(MAX(ctch.rarebonus), '~', ctch.Basic, '~', ctch.perkg) AS grp
                 FROM shore.dbo.tblCatch AS ctch
                      INNER JOIN shore.dbo.tblComps AS cmp ON ctch.CompID = cmp.CompID
                      INNER JOIN shore.dbo.tblSeasons sn ON cmp.SeasonID = sn.SeasonID
                      INNER JOIN shore.dbo.tblSpecies spc ON ctch.SpeciesID = spc.SpeciesID
                 GROUP BY ctch.SpeciesID,
                          spc.Species,
                          ctch.Basic,
                          ctch.perkg,
                          spc.BonusWeight,
                          Season
             ) AS t1
         ) t2
     ) t3
     GROUP BY SpeciesID,
              Species,
              rarebonus,
              Basic,
              perkg,
              BonusWeight,
              grp;