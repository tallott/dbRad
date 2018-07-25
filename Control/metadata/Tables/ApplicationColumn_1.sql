CREATE TABLE [metadata].[ApplicationColumn] (
    [ApplicationColumnId]  INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationTableId]   INT            NULL,
    [ColumnName]           VARCHAR (128)  NULL,
    [ColumnLable]          VARCHAR (128)  NULL,
    [RowSource]            VARCHAR (4000) NULL,
    [Filter]               VARCHAR (128)  NULL,
    [OrderBy]              VARCHAR (128)  NULL,
    [WindowControlTypeId]  INT            NULL,
    [WindowControlEnabled] BIT            NULL,
    [WindowLayoutOrder]    INT            NULL,
    CONSTRAINT [PK__Applicat__4A666BFB8891CDBF] PRIMARY KEY CLUSTERED ([ApplicationColumnId] ASC),
    CONSTRAINT [FK_ApplicationColumn_ApplicationTable] FOREIGN KEY ([ApplicationTableId]) REFERENCES [metadata].[ApplicationTable] ([ApplicationTableId]),
    CONSTRAINT [FK_ApplicationColumn_WindowControlType] FOREIGN KEY ([WindowControlTypeId]) REFERENCES [metadata].[WindowControlType] ([WindowControlTypeId])
);

