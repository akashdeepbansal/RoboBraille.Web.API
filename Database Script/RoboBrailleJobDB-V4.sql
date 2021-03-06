USE [RoboBrailleJobDB]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 1/13/2017 4:10:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Jobs]    Script Date: 1/13/2017 4:10:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Jobs](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](512) NOT NULL,
	[FileExtension] [nvarchar](512) NOT NULL,
	[MimeType] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[FinishTime] [datetime] NOT NULL,
	[InputFileHash] [varbinary](max) NULL,
	[ResultContent] [varbinary](max) NULL,
	[DownloadCounter] [int] NOT NULL,
	[ResultFileExtension] [nvarchar](max) NULL,
	[ResultMimeType] [nvarchar](max) NULL,
	[TargetDocumentFormat] [int] NULL,
	[FormatOptions] [int] NULL,
	[OutputFormat] [int] NULL,
	[DaisyOutput] [int] NULL,
	[EbookFormat] [int] NULL,
	[MSOfficeOutput] [int] NULL,
	[VideoUrl] [nvarchar](max) NULL,
	[SubtitleLangauge] [nvarchar](max) NULL,
	[SubtitleFormat] [nvarchar](max) NULL,
	[AmaraVideoId] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.Jobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServiceUsers]    Script Date: 1/13/2017 4:10:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServiceUsers](
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](32) NOT NULL,
	[ApiKey] [varbinary](max) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NOT NULL,
	[EmailAddress] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.ServiceUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Jobs] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[ServiceUsers] ADD  DEFAULT (newsequentialid()) FOR [UserId]
GO
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Jobs_dbo.ServiceUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[ServiceUsers] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_dbo.Jobs_dbo.ServiceUsers_UserId]
GO
