CREATE TABLE [dbo].[ApplicationFilter] (
    [ApplicationFilterId] INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationTableId]  INT            NULL,
    [FilterName]          VARCHAR (50)   NULL,
    [FilterDefinition]    VARCHAR (8000) NOT NULL,
    [SortOrder]           INT            NOT NULL,
    CONSTRAINT [PK_ApplicationFilter] PRIMARY KEY CLUSTERED ([ApplicationFilterId] ASC),
    CONSTRAINT [FK_ApplicationFilter_ApplicationTable] FOREIGN KEY ([ApplicationTableId]) REFERENCES [dbo].[ApplicationTable] ([ApplicationTableId])
);







