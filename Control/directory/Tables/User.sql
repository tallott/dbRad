CREATE TABLE [directory].[User] (
    [UserId]        INT          IDENTITY (1, 1) NOT NULL,
    [UserName]      VARCHAR (50) NOT NULL,
    [UserFirstName] VARCHAR (50) NULL,
    [UserLastName]  VARCHAR (50) NULL,
    [UserPassword]  VARCHAR (50) NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

