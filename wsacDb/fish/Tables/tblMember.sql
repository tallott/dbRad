CREATE TABLE [fish].[tblMember] (
    [MemberID]    INT            IDENTITY (1, 1) NOT NULL,
    [GenderID]    INT            NULL,
    [Lastname]    NVARCHAR (255) NULL,
    [Firstname]   NVARCHAR (255) NULL,
    [DateOfBirth] DATE           NULL,
    CONSTRAINT [PK_tblMembers] PRIMARY KEY CLUSTERED ([MemberID] ASC),
    CONSTRAINT [FK_tblMember_tblGender] FOREIGN KEY ([GenderID]) REFERENCES [fish].[tblGender] ([GenderID])
);



