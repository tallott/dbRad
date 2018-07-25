CREATE TABLE [metadata].[Application] (
    [ApplicationId]   INT          IDENTITY (1, 1) NOT NULL,
    [ApplicationName] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([ApplicationId] ASC)
);

