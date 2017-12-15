CREATE TABLE [fish].[tblSpecie] (
    [SpecieID] INT            IDENTITY (1, 1) NOT NULL,
    [Species]  NVARCHAR (255) NULL,
    [ClubName] NCHAR (10)     NULL,
    [Type]     NVARCHAR (1)   NULL,
    CONSTRAINT [PK_tblSpecies] PRIMARY KEY CLUSTERED ([SpecieID] ASC)
);

