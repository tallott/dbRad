CREATE procedure [dbo].[updates] as 

--Shore Memeber Update

Update shore.dbo.tblMembers set FirstName = 'Murray'
where MemberId = 23

Update shore.dbo.tblMembers set FirstName = 'Robby'
where MemberId = 127

Update shore.dbo.tblMembers set FirstName = 'Samantha'
where MemberId = 112


Update shore.dbo.tblMembers set LastName = 'Grocott'
where MemberId =77

Update shore.dbo.tblMembers set LastName = 'Hendriksen'
where MemberId = 119


Update shore.dbo.tblMembers set LastName = 'Jeffrey'
where MemberId = 234


Update shore.dbo.tblCatch set MemberID = 117
where MemberId  = 260

delete from shore.dbo.tblMembers where MemberID = 260


Update shore.dbo.tblCatch set MemberID = 99
where MemberId  = 262

delete from shore.dbo.tblMembers where MemberID = 262

Update shore.dbo.tblMembers set LastName = 'Renata'
where MemberId = 99

--Boat Memebers Update


Update boat.dbo.tblMembers set FirstName = 'Alistair'
where MemberId =47



--Move comps to correct season.

update shore.dbo.tblComps
set SeasonID = 7 where SeasonID = 16 and CompID in(78,79)

--correct comp numbers

update shore.dbo.tblComps set Number = 1 where compID = 232
update shore.dbo.tblComps set Number = 2 where compID = 233
update shore.dbo.tblComps set Number = 3 where compID = 234
update shore.dbo.tblComps set Number = 4 where compID = 235
update shore.dbo.tblComps set Number = 5 where compID = 236
update shore.dbo.tblComps set Number = 6 where compID = 237
update shore.dbo.tblComps set Number = 7 where compID = 238
update shore.dbo.tblComps set Number = 8 where compID = 239
update shore.dbo.tblComps set Number = 9 where compID = 240
update shore.dbo.tblComps set Number = 10 where compID = 241
update shore.dbo.tblComps set Number = 11 where compID = 242
update shore.dbo.tblComps set Number = 12 where compID = 243
update shore.dbo.tblComps set number = 1 where CompID = 78
update shore.dbo.tblComps set number = 2 where CompID = 79

--Delete comp that should not be here
delete from shore.dbo.tblComps where CompID = 293

--correct catch points 
update shore.dbo.tblCatch set Basic = 20 where CatchID = 34283
update shore.dbo.tblCatch set Basic = 15 where CatchID =34282

update shore.dbo.tblCatch set Basic = 20 where CatchID IN(3761,3802,4180,4201,4207,4246,4247)

update shore.dbo.tblCatch set Basic = 5 where CatchID =30623
--delete Xtra Season
delete from shore.dbo.tblSeasons where SeasonID = 21
--Rename Season
update shore.dbo.tblSeasons
set Season = '1998/1999' where season = '98/99'

update shore.dbo.tblSeasons
set Season = '1999/2000' where season = '99/00'


update shore.dbo.tblSeasons
set Season = '2000/2001' where season = '00/01'

update shore.dbo.tblSeasons
set Season = '2001/2002' where season = '01/02'

update shore.dbo.tblSeasons
set Season = '2002/2002' where season ='02/03'

update shore.dbo.tblSeasons
set Season = '2003/2004' where season ='03/04'

update shore.dbo.tblSeasons
set Season = '2004/2005' where season ='04/05'

update shore.dbo.tblSeasons
set Season = '2005/2006' where season ='05/06'

update shore.dbo.tblSeasons
set Season = '2006/2007' where season ='06/07'

update shore.dbo.tblSeasons
set Season = '2007/2008' where season ='07/08'

update shore.dbo.tblSeasons
set Season = '2008/2009' where season ='08/09'

update shore.dbo.tblSeasons
set Season = '2009/2010' where season ='09/10'

update shore.dbo.tblSeasons
set Season = '2010/2011' where season ='10/11'

update shore.dbo.tblSeasons
set Season = '2011/2012' where season ='11/12'

update shore.dbo.tblSeasons
set Season = '2012/2013' where season ='12/13'

update shore.dbo.tblSeasons
set Season = '2013/2014' where season ='13/14'

update shore.dbo.tblSeasons
set Season = '2014/2015' where season ='14/15'

update shore.dbo.tblSeasons
set Season = '2015/2016' where season ='15/16'


--Pairs shore
DELETE from shore.dbo.tblAnglerPair where PairID is NULL
DELETE from shore.dbo.tblAnglerPair where AnglerID is NULL

DELETE from shore.dbo.tblAnglerPair where AnglerPairID = 261
DELETE from shore.dbo.tblAnglerPair where AnglerPairID = 262

DELETE from shore.dbo.tblAnglerPair where PairId in (select PairID from shore.dbo.tblAnglerPair group by PairID having count(*)  = 1)
 --Pairs Boat
 --Pairs
DELETE from boat.dbo.tblAnglerPair where PairID is NULL

DELETE from boat.dbo.tblAnglerPair where AnglerID is NULL


DELETE from boat.dbo.tblAnglerPair where AnglerPairID = 689
DELETE from boat.dbo.tblAnglerPair where AnglerPairID = 690

DELETE from boat.dbo.tblAnglerPair where PairId in (select PairID from boat.dbo.tblAnglerPair group by PairID having count(*)  = 1)
--Species
--TO DO***TO DO***TO DO***TO DO***TO DO***TO DO***TO DO***TO DO***