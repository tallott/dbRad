CREATE TABLE [dbo].[ApplicationColumn] (
    [ApplicationColumnId]  INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationTableId]   INT            NULL,
    [ColumnName]           VARCHAR (128)  NULL,
    [ColumnLable]          VARCHAR (128)  NULL,
    [RowSource]            VARCHAR (4000) NULL,
    [WindowControlTypeId]  INT            NULL,
    [WindowControlEnabled] BIT            NULL,
    [WindowLayoutOrder]    INT            NULL,
    CONSTRAINT [PK__Applicat__4A666BFB8891CDBF] PRIMARY KEY CLUSTERED ([ApplicationColumnId] ASC),
    CONSTRAINT [FK_ApplicationColumn_ApplicationTable] FOREIGN KEY ([ApplicationTableId]) REFERENCES [dbo].[ApplicationTable] ([ApplicationTableId]),
    CONSTRAINT [FK_ApplicationColumn_WindowControlType] FOREIGN KEY ([WindowControlTypeId]) REFERENCES [dbo].[WindowControlType] ([WindowControlTypeId])
);













