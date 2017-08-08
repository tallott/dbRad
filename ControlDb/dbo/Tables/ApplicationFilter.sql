CREATE TABLE [dbo].[ApplicationFilter] (
    [ApplicationFilterId] INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationTableId]  INT            NULL,
    [FilterName]          VARCHAR (50)   NULL,
    [FilterDefinition]    VARCHAR (4000) NOT NULL,
    [SortOrder]           INT            NOT NULL,
    CONSTRAINT [PK__tmp_ms_x__0B6EC049E5EF9A64] PRIMARY KEY CLUSTERED ([ApplicationFilterId] ASC),
    CONSTRAINT [FK_ApplicationFilter_ApplicationTable] FOREIGN KEY ([ApplicationTableId]) REFERENCES [dbo].[ApplicationTable] ([ApplicationTableId])
);









