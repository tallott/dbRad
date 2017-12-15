CREATE TABLE [fish].[tblComp] (
    [CompID]    INT            IDENTITY (1, 1) NOT NULL,
    [SectionID] INT            NOT NULL,
    [SeasonID]  INT            NULL,
    [Number]    TINYINT        NULL,
    [Start]     DATE           NULL,
    [Finish]    DATE           NULL,
    [Report]    NVARCHAR (MAX) NULL,
    [Tide]      NVARCHAR (50)  NULL,
    [Moon]      NVARCHAR (50)  NULL,
    [Weather]   NVARCHAR (50)  NULL,
    CONSTRAINT [PK_tblComps] PRIMARY KEY CLUSTERED ([CompID] ASC),
    CONSTRAINT [FK_tblComp_tblSection] FOREIGN KEY ([SectionID]) REFERENCES [fish].[tblSection] ([SectionID]),
    CONSTRAINT [FK_tblComps_tblSeasons] FOREIGN KEY ([SeasonID]) REFERENCES [fish].[tblSeason] ([SeasonID])
);



