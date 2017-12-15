CREATE VIEW Champion
AS
     SELECT concat(Lastname, '~', Firstname) AS FullName
     FROM shore.dbo.tblCoC coc
          INNER JOIN shore.dbo.tblMembers mbr ON mbr.MemberID = coc.MemberID;