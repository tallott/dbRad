CREATE TABLE [fish].[tblSectionPointsRule] (
    [SectionPointsRuleID] INT           IDENTITY (1, 1) NOT NULL,
    [SectionID]           INT           NULL,
    [RuleGroup]           VARCHAR (10)  NULL,
    [RuleMacro]           VARCHAR (255) NULL,
    [ValidFromDate]       DATE          NULL,
    [ValidToDate]         DATE          NULL,
    CONSTRAINT [PK_tblSectionPointsRule] PRIMARY KEY CLUSTERED ([SectionPointsRuleID] ASC),
    CONSTRAINT [FK_tblSectionPointsRule_tblSection] FOREIGN KEY ([SectionID]) REFERENCES [fish].[tblSection] ([SectionID])
);



