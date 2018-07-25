CREATE TABLE [metadata].[WindowObjectText] (
    [WindowObjectTextId]  INT          IDENTITY (1, 1) NOT NULL,
    [ApplicationColumnId] INT          NULL,
    [ObjectTextName]      VARCHAR (50) NULL,
    [ApplicationWindowId] VARCHAR (50) NULL,
    [ObjectTextId]        INT          NULL,
    [WindowControlType]   VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([WindowObjectTextId] ASC)
);

