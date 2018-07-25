CREATE TABLE [metadata].[ApplicationSchema] (
    [ApplicationSchemaId] INT IDENTITY (1, 1) NOT NULL,
    [ApplicationId]       INT NULL,
    [SchemaId]            INT NULL,
    CONSTRAINT [PK__Applicat__130622AA0228D1BC] PRIMARY KEY CLUSTERED ([ApplicationSchemaId] ASC),
    CONSTRAINT [FK_ApplicationSchema_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [metadata].[Application] ([ApplicationId]),
    CONSTRAINT [FK_ApplicationSchema_Schema] FOREIGN KEY ([SchemaId]) REFERENCES [metadata].[Schema] ([SchemaId])
);

