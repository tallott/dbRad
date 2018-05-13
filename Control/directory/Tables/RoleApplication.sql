CREATE TABLE [directory].[RoleApplication] (
    [RoleApplicationId] INT IDENTITY (1, 1) NOT NULL,
    [RoleId]            INT NOT NULL,
    [ApplicationId]     INT NOT NULL,
    CONSTRAINT [PK__RoleApplication] PRIMARY KEY CLUSTERED ([RoleApplicationId] ASC),
    CONSTRAINT [FK_RoleApplication_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [metadata].[Application] ([ApplicationId]),
    CONSTRAINT [FK_RoleApplication_Role1] FOREIGN KEY ([RoleId]) REFERENCES [directory].[Role] ([RoleId])
);

