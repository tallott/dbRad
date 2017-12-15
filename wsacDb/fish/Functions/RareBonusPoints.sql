CREATE FUNCTION [fish].[RareBonusPoints](@CatchID INT)
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
                                                       AND cmp.SectionId = sp.SectionID
                                                       AND cmp.Start BETWEEN sp.ValidFromDate AND isnull(sp.ValidToDate, GETDATE())
         WHERE c.CatchID = @CatchID;
         RETURN @RareBonus;
     END;