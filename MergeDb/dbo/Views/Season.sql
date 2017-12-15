create view Season
as
SELECT Season,
       S_SeasonID,
        B_SeasonID
FROM
(
    SELECT COALESCE(s.Season, b.Season) AS Season,
           s.SeasonID as S_SeasonID,
           b.SeasonID as B_SeasonID
    FROM shore.dbo.tblSeasons s
         FULL OUTER JOIN boat.dbo.tblSeasons b ON s.Season = b.Season
) t1