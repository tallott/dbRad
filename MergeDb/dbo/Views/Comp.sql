CREATE VIEW [dbo].[Comp]
AS
     SELECT c.CompID AS S_CompID,
            NULL AS B_CompID,
            c.SeasonID AS S_SeasonID,
            NULL AS B_SeasonID,
            s.Season,
            c.Number,
            CONVERT(DATE, c.Start) AS Start,
            CONVERT(DATE, c.Finish) AS Finish,
            c.Report,
            c.Tide,
            c.Moon,
            c.Weather
     FROM shore.dbo.tblComps c
          INNER JOIN shore.dbo.tblSeasons s ON s.SeasonID = c.SeasonID
     UNION
     SELECT NULL AS S_CompID,
            c.CompID,
            NULL,
            c.SeasonID,
            s.Season,
            c.Number,
            CONVERT(DATE, c.Start) AS Start,
            CONVERT(DATE, c.Finish) AS Finish,
            c.Report,
            CASE
                WHEN c.WTide IS NULL
                     AND c.PTide IS NULL
                THEN NULL
                WHEN c.WTide IS NOT NULL
                     AND c.PTide IS NOT NULL
                THEN concat('Wellington: ', c.WTide, CHAR(13), CHAR(13), 'Porirua: ', c.PTide)
                WHEN c.WTide IS NOT NULL
                THEN concat('Wellington: ', c.WTide)
                ELSE concat('Porirua: ', c.PTide)
            END AS Tide,
            c.Moon,
            c.Weather
     FROM boat.dbo.tblComps c
          INNER JOIN boat.dbo.tblSeasons s ON s.SeasonID = c.SeasonID;