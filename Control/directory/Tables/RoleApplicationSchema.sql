CREATE TABLE [directory].[RoleApplicationSchema] (
    [RoleApplicationSchemaId] INT IDENTITY (1, 1) NOT NULL,
    [RoleId]                  INT NOT NULL,
    [ApplicationSchemaId]     INT NOT NULL,
    CONSTRAINT [PK__RoleApplicationShema] PRIMARY KEY CLUSTERED ([RoleApplicationSchemaId] ASC),
    CONSTRAINT [FK_RoleApplicationSchema_ApplicationSchema] FOREIGN KEY ([ApplicationSchemaId]) REFERENCES [metadata].[ApplicationSchema] ([ApplicationSchemaId]),
    CONSTRAINT [FK_RoleApplicationSchema_Role] FOREIGN KEY ([RoleId]) REFERENCES [directory].[Role] ([RoleId])
);

