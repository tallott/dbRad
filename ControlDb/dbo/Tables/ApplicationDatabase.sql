CREATE TABLE [dbo].[ApplicationDatabase] (
    [ApplicationDatabaseId] INT          IDENTITY (1, 1) NOT NULL,
    [DatabaseName]          VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ApplicationDatabase] PRIMARY KEY CLUSTERED ([ApplicationDatabaseId] ASC)
);



