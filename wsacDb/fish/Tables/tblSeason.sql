CREATE TABLE [fish].[tblSeason] (
    [SeasonID]  INT           IDENTITY (1, 1) NOT NULL,
    [Season]    NVARCHAR (50) NULL,
    [IsCurrent] BIT           CONSTRAINT [DF_tblSeason_IsCurrent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_tblSeasons] PRIMARY KEY CLUSTERED ([SeasonID] ASC)
);





