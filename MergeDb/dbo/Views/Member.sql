create view
Member
as

SELECT S_MemberId,B_MemberId,
       LastName,
       Firstname
FROM
(
    SELECT s.memberid AS S_MemberId,
    b.memberid AS B_MemberId,
           COALESCE(s.LastName, b.LastName) AS LastName,
           COALESCE(s.Firstname, b.Firstname) AS Firstname,
           s.LastName s,
           b.LastName b
    FROM shore.dbo.tblMembers s
         FULL OUTER JOIN boat.dbo.tblMembers b ON s.Lastname = b.Lastname
                                                  AND s.Firstname = b.Firstname
) t1