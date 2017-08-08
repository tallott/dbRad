CREATE TABLE [dbo].[ApplicationTable] (
    [ApplicationTableId]    INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationDatabaseId] INT            NULL,
    [ApplicationSchemaId]   INT            NULL,
    [TableName]             VARCHAR (128)  NOT NULL,
    [TableLabel]            VARCHAR (4000) NULL,
    [Dml]                   VARCHAR (4000) NULL,
    [TableKey]              VARCHAR (128)  NULL,
    CONSTRAINT [PK__tmp_ms_x__003EC42C20858B67] PRIMARY KEY CLUSTERED ([ApplicationTableId] ASC),
    CONSTRAINT [FK_ApplicationTable_ApplicationDatabase] FOREIGN KEY ([ApplicationDatabaseId]) REFERENCES [dbo].[ApplicationDatabase] ([ApplicationDatabaseId]),
    CONSTRAINT [FK_ApplicationTable_ApplicationSchema] FOREIGN KEY ([ApplicationSchemaId]) REFERENCES [dbo].[ApplicationSchema] ([ApplicationSchemaId])
);















