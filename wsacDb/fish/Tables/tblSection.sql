CREATE TABLE [fish].[tblSection] (
    [SectionID] INT           IDENTITY (1, 1) NOT NULL,
    [Section]   NVARCHAR (20) NULL,
    CONSTRAINT [PK_tblSection] PRIMARY KEY CLUSTERED ([SectionID] ASC)
);

