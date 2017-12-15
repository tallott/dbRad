CREATE TABLE [fish].[tblSpeciePoints] (
    [SpeciePointsID]    INT        IDENTITY (1, 1) NOT NULL,
    [SpecieID]          INT        NOT NULL,
    [SectionID]         INT        NOT NULL,
    [BasicPoints]       FLOAT (53) NOT NULL,
    [PerKgPoints]       FLOAT (53) NOT NULL,
    [RareBonusPoints]   FLOAT (53) NULL,
    [WeightBonusPoints] FLOAT (53) NULL,
    [ValidFromDate]     DATE       NOT NULL,
    [ValidToDate]       DATE       NULL,
    CONSTRAINT [PK_tblSpeciesPoints] PRIMARY KEY CLUSTERED ([SpeciePointsID] ASC),
    CONSTRAINT [FK_tblSpeciePoints_tblSection] FOREIGN KEY ([SectionID]) REFERENCES [fish].[tblSection] ([SectionID]),
    CONSTRAINT [FK_tblSpeciesPoints_tblSpecie] FOREIGN KEY ([SpecieID]) REFERENCES [fish].[tblSpecie] ([SpecieID])
);

