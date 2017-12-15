create FUNCTION [fish].[BasicPoints](@CatchID INT)
RETURNS INT
AS
     BEGIN
         DECLARE @Basic INT;
         SELECT @Basic = sp.BasicPoints
         FROM fish.tblCatch AS c
              INNER JOIN fish.tblComp AS cmp ON c.CompID = cmp.CompID
              INNER JOIN fish.tblSpeciePoints AS sp ON c.SpecieID = sp.SpecieID
                                                       AND cmp.SectionId = sp.SectionID
                                                       AND cmp.Start BETWEEN sp.ValidFromDate AND isnull(sp.ValidToDate, GETDATE())
         WHERE c.CatchID = @CatchID;
         RETURN @Basic;
     END;