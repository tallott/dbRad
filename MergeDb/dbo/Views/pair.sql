

CREATE view [dbo].[pair] as
SELECT PairId AS S_PairID,
       NULL AS b_PairID,
       Season,
	  [desc],
       MemberId1,
       concat(m1.Lastname, '~', m1.Firstname) AS FullName1,
       MemberId2,
       concat(m2.Lastname, '~', m2.Firstname) AS FullName2
FROM
(
    SELECT ap.PairId,
           ssn.Season,
           p.[desc],
           MIN(AnglerId) AS MemberId1,
           MAX(AnglerId) AS MemberId2
    FROM shore.dbo.tblAnglerPair ap
         INNER JOIN shore.dbo.tblPairs p ON p.PairID = ap.PairID
         INNER JOIN shore.dbo.tblSeasons ssn ON ssn.SeasonID = p.SeasonID
    GROUP BY ap.PairId,
             ssn.Season,
             p.[Desc]
) t1
INNER JOIN shore.dbo.tblMembers m1 ON m1.MemberID = t1.MemberId1
INNER JOIN shore.dbo.tblMembers m2 ON m2.MemberID = t1.MemberId2
UNION
SELECT NULL AS S_PairID,
       PairId AS b_PairID,
       Season,
	  [desc],
       MemberId1,
       concat(m1.Lastname, '~', m1.Firstname) AS FullName1,
       MemberId2,
       concat(m2.Lastname, '~', m2.Firstname) AS FullName2
FROM
(
    SELECT ap.PairId,
           ssn.Season,
           p.[desc],
           MIN(AnglerId) AS MemberId1,
           MAX(AnglerId) AS MemberId2
    FROM boat.dbo.tblAnglerPair ap
         INNER JOIN boat.dbo.tblPairs p ON p.PairID = ap.PairID
         INNER JOIN boat.dbo.tblSeasons ssn ON ssn.SeasonID = p.SeasonID
    GROUP BY ap.PairId,
             ssn.Season,
             p.[Desc]
) t1
INNER JOIN boat.dbo.tblMembers m1 ON m1.MemberID = t1.MemberId1
INNER JOIN boat.dbo.tblMembers m2 ON m2.MemberID = t1.MemberId2;