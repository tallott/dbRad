CREATE TABLE [metadata].[ApplicationTableRelation] (
    [ApplicationTableRelationId] INT IDENTITY (1, 1) NOT NULL,
    [ParentColumnID]             INT NULL,
    [ChildColumnId]              INT NULL,
    PRIMARY KEY CLUSTERED ([ApplicationTableRelationId] ASC)
);

