CREATE TABLE [fish].[tblCatch] (
    [CatchID]  INT        IDENTITY (1, 1) NOT NULL,
    [SpecieID] INT        NULL,
    [MemberID] INT        NOT NULL,
    [CompID]   INT        NOT NULL,
    [Weight]   FLOAT (53) NOT NULL,
    [BSL]      FLOAT (53) NULL,
    [Score]    FLOAT (53) NULL,
    CONSTRAINT [PK_tblCatch] PRIMARY KEY CLUSTERED ([CatchID] ASC),
    CONSTRAINT [FK_tblCatch_tblComps] FOREIGN KEY ([CompID]) REFERENCES [fish].[tblComp] ([CompID]),
    CONSTRAINT [FK_tblCatch_tblMembers] FOREIGN KEY ([MemberID]) REFERENCES [fish].[tblMember] ([MemberID]),
    CONSTRAINT [FK_tblCatch_tblSpecie] FOREIGN KEY ([SpecieID]) REFERENCES [fish].[tblSpecie] ([SpecieID])
);






GO
CREATE NONCLUSTERED INDEX [IX_tblCatch]
    ON [fish].[tblCatch]([CatchID] ASC);


GO
-- =============================================
-- Author:		Tim Allott
-- Create date: 
-- Description:	Calculates Catch record points
-- =============================================
CREATE TRIGGER [fish].[CalcPoints] ON [fish].[tblCatch]
AFTER INSERT, UPDATE
AS
     UPDATE t
       SET
           Score =fish.BasicPoints(i.CatchId) + fish.lineBonusPoints(i.catchId) + fish.RareBonusPoints(i.CatchID)
     FROM fish.tblCatch t
          INNER JOIN inserted i ON t.CatchId = i.CatchId;
