CREATE TABLE [fish].[tblChampion] (
    [ChampionID] INT IDENTITY (1, 1) NOT NULL,
    [MemberID]   INT NULL,
    CONSTRAINT [PK_tblChampion] PRIMARY KEY CLUSTERED ([ChampionID] ASC),
    CONSTRAINT [FK_tblCoC_tblMembers] FOREIGN KEY ([MemberID]) REFERENCES [fish].[tblMember] ([MemberID])
);

