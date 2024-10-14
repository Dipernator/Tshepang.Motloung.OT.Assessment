USE [OT_Assessment_DB]
GO

/****** Object:  Table [dbo].[CasinoWager]    Script Date: 2024/10/14 08:08:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CasinoWager](
	[WagerId] [uniqueidentifier] NOT NULL,
	[Theme] [nvarchar](max) NOT NULL,
	[Provider] [nvarchar](450) NOT NULL,
	[GameName] [nvarchar](max) NOT NULL,
	[TransactionId] [uniqueidentifier] NOT NULL,
	[BrandId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](max) NOT NULL,
	[ExternalReferenceId] [uniqueidentifier] NOT NULL,
	[TransactionTypeId] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[NumberOfBets] [int] NOT NULL,
	[CountryCode] [nvarchar](max) NOT NULL,
	[SessionData] [nvarchar](max) NOT NULL,
	[Duration] [bigint] NOT NULL,
 CONSTRAINT [PK_CasinoWager] PRIMARY KEY CLUSTERED 
(
	[WagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


