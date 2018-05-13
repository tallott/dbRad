CREATE TABLE [metadata].[Schema] (
    [SchemaId]    INT          IDENTITY (1, 1) NOT NULL,
    [SchemaName]  VARCHAR (50) NOT NULL,
    [SchemaLabel] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([SchemaId] ASC)
);

