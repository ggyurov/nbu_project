SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- USERS table
CREATE TABLE [dbo].[Users]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[NormalizedUserName] [nvarchar](256) NOT NULL,
	[Role] INT NOT NULL,
	[City] [nvarchar](256) NULL,
	[Name] [nvarchar](256) NULL,
	[Type] [nvarchar](256) NULL,
CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO

-- APPOINTMENTS table
CREATE TABLE [dbo].[Appointments]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[Date] DATETIME NOT NULL,
	[DoctorId] INT NOT NULL,
	[CanceledById] INT NULL,
	[CanceledOn] DATETIME NULL,
CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO

-- RATINGS table
CREATE TABLE [dbo].[Ratings]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[ByUserId] INT NOT NULL,
	[Value] INT NOT NULL,
CONSTRAINT [PK_Ratings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO

-- COMMENTS table
CREATE TABLE [dbo].[Comments]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Text] NVARCHAR(256) NOT NULL,
	[ByUserId] INT NOT NULL,
	[UserId] INT NULL,
	[EventId] INT NULL,
CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO

-- EVENTS table
CREATE TABLE [dbo].[Events]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[Title] NVARCHAR(128) NOT NULL,
	[Text] NVARCHAR(256) NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO

-- WorkingHours table
CREATE TABLE [dbo].[WorkingHours]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[DayOfWeek] INT NOT NULL,
	[StartTime] DATETIME NOT NULL,
	[EndTime] DATETIME NOT NULL,
CONSTRAINT [PK_WorkingHours] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
	WITH
	(
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
)
GO
-- Foreign keys
ALTER TABLE [dbo].[Appointments] WITH CHECK
ADD CONSTRAINT [FK_Appointments_Users_UserId] FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Appointments]
CHECK CONSTRAINT [FK_Appointments_Users_UserId]
GO

ALTER TABLE [dbo].[Appointments] WITH CHECK
ADD CONSTRAINT [FK_Appointments_Users_DoctorId] FOREIGN KEY ([DoctorId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Appointments]
CHECK CONSTRAINT [FK_Appointments_Users_DoctorId]
GO

ALTER TABLE [dbo].[Appointments] WITH CHECK
ADD CONSTRAINT [FK_Appointments_Users_CanceledById] FOREIGN KEY ([CanceledById])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Appointments]
CHECK CONSTRAINT [FK_Appointments_Users_CanceledById]
GO

ALTER TABLE [dbo].[Ratings] WITH CHECK
ADD CONSTRAINT [FK_Ratings_Users_UserId] FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Ratings]
CHECK CONSTRAINT [FK_Ratings_Users_UserId]
GO

ALTER TABLE [dbo].[Ratings] WITH CHECK
ADD CONSTRAINT [FK_Ratings_Users_ByUserId] FOREIGN KEY ([ByUserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Ratings]
CHECK CONSTRAINT [FK_Ratings_Users_ByUserId]
GO

ALTER TABLE [dbo].[Comments] WITH CHECK
ADD CONSTRAINT [FK_Comments_Users_UserId] FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Comments]
CHECK CONSTRAINT [FK_Comments_Users_UserId]
GO

ALTER TABLE [dbo].[Comments] WITH CHECK
ADD CONSTRAINT [FK_Comments_Users_ByUserId] FOREIGN KEY ([ByUserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Comments]
CHECK CONSTRAINT [FK_Comments_Users_ByUserId]
GO

ALTER TABLE [dbo].[Comments] WITH CHECK
ADD CONSTRAINT [FK_Comments_Events_EventId] FOREIGN KEY ([EventId])
REFERENCES [dbo].[Events] ([Id])
GO

ALTER TABLE [dbo].[Comments]
CHECK CONSTRAINT [FK_Comments_Events_EventId]
GO

ALTER TABLE [dbo].[Events] WITH CHECK
ADD CONSTRAINT [FK_Events_Users_UserId] FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Events]
CHECK CONSTRAINT [FK_Events_Users_UserId]
GO

ALTER TABLE [dbo].[WorkingHours] WITH CHECK
ADD CONSTRAINT [FK_WorkingHours_Users_UserId] FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[WorkingHours]
CHECK CONSTRAINT [FK_WorkingHours_Users_UserId]
GO