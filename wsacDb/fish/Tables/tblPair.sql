CREATE TABLE [fish].[tblPair] (
    [PairID]          INT           IDENTITY (1, 1) NOT NULL,
    [PairDescription] NVARCHAR (50) NULL,
    [SeasonID]        INT           NULL,
    [SectionID]       INT           NULL,
    [MemberID1]       INT           NULL,
    [MemberID2]       INT           NULL,
    CONSTRAINT [PK_tblPairs] PRIMARY KEY CLUSTERED ([PairID] ASC),
    CONSTRAINT [FK_tblPairs_tblSeasons] FOREIGN KEY ([SeasonID]) REFERENCES [fish].[tblSeason] ([SeasonID])
);

