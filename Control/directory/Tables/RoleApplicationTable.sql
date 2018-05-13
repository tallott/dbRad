CREATE TABLE [directory].[RoleApplicationTable] (
    [RoleApplicationTableId] INT IDENTITY (1, 1) NOT NULL,
    [RoleId]                 INT NOT NULL,
    [ApplicationTableId]     INT NOT NULL,
    CONSTRAINT [PK__RoleApplicationTable] PRIMARY KEY CLUSTERED ([RoleApplicationTableId] ASC),
    CONSTRAINT [FK_RoleApplicationTable_ApplicationTable1] FOREIGN KEY ([ApplicationTableId]) REFERENCES [metadata].[ApplicationTable] ([ApplicationTableId]),
    CONSTRAINT [FK_RoleApplicationTable_Role] FOREIGN KEY ([RoleId]) REFERENCES [directory].[Role] ([RoleId])
);

