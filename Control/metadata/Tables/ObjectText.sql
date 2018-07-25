CREATE TABLE [metadata].[ObjectText] (
    [ObjectTextId]      INT           IDENTITY (1, 1) NOT NULL,
    [WindowControlType] VARCHAR (50)  NULL,
    [ObjectText]        VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([ObjectTextId] ASC)
);

