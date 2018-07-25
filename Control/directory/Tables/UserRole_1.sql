CREATE TABLE [directory].[UserRole] (
    [UserRoleId] INT IDENTITY (1, 1) NOT NULL,
    [UserId]     INT NULL,
    [RoleId]     INT NULL,
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [directory].[Role] ([RoleId]),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [directory].[User] ([UserId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ApllicationUserRole_uk1]
    ON [directory].[UserRole]([UserId] ASC, [RoleId] ASC);

