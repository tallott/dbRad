USE [Control]
GO
/****** Object:  Table [dbo].[WindowControlType]    Script Date: 14/06/2017 5:40:42 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WindowControlType](
	[WindowControlTypeId] [int] IDENTITY(1,1) NOT NULL,
	[WindowControlType] [varchar](50) NOT NULL,
 CONSTRAINT [PK_WindowControlType] PRIMARY KEY CLUSTERED 
(
	[WindowControlTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[WindowControlType] ON 

INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (1, N'ID')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (2, N'TEXT')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (3, N'TEXTBLOCK')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (4, N'NUM')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (5, N'CHK')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (6, N'DATE')
INSERT [dbo].[WindowControlType] ([WindowControlTypeId], [WindowControlType]) VALUES (7, N'COMBO')
SET IDENTITY_INSERT [dbo].[WindowControlType] OFF
