CREATE TABLE [fish].[tblGender] (
    [GenderID] INT          IDENTITY (1, 1) NOT NULL,
    [Gender]   VARCHAR (10) NULL,
    CONSTRAINT [PK_tblGender] PRIMARY KEY CLUSTERED ([GenderID] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [tblGender_uk1]
    ON [fish].[tblGender]([Gender] ASC);

