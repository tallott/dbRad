CREATE FUNCTION [fish].[LineBonusPoints](@CatchID INT)
RETURNS FLOAT
AS
     BEGIN
         DECLARE @result FLOAT;
         SELECT @result = CASE sectionId
                              WHEN 1
                              THEN FLOOR((Ratio / 0.5) - 1) * BasicPoints
                              WHEN 2
                              THEN CASE Type
                                       WHEN 'S'
                                       THEN CASE
                                                WHEN Ratio >= 0.5
                                                THEN IntPart + CASE
                                                                   WHEN Ratio - IntPart >= 0.5
                                                                   THEN 0.5
                                                                   ELSE 0
                                                               END / 0.5 * BasicPoints
                                                ELSE 0
                                            END
                                       WHEN 'N'
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
                          END
         FROM
         (
             SELECT sp.BasicPoints,
                    cmp.SectionID,
                    s.Type,
                    c.weight / c.BSL AS Ratio,
                    FLOOR(c.weight / c.BSL) AS IntPart
             FROM fish.tblCatch AS c
                  INNER JOIN fish.tblComp AS cmp ON c.CompID = cmp.CompID
                  INNER JOIN fish.tblSpecie AS s ON c.SpecieID = s.SpecieID
                  INNER JOIN fish.tblSpeciePoints AS sp ON s.SpecieID = sp.SpecieID
                                                           AND cmp.SectionId = sp.SectionID
                                                           AND cmp.Start BETWEEN sp.ValidFromDate AND isnull(sp.ValidToDate, GETDATE())
             WHERE c.CatchID = @CatchID
         ) AS t1;
         RETURN @result;
     END;