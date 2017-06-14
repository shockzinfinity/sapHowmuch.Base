CREATE TABLE [dbo].[system_logging] (
	[system_logging_guid] [UNIQUEIDENTIFIER] ROWGUIDCOL NOT NULL
	, [entered_date] [DATETIME] NULL
	, [log_application] [NVARCHAR](200) NULL
	, [log_date] [NVARCHAR](100) NULL
	, [log_level] [NVARCHAR](100) NULL
	, [log_logger] [NVARCHAR](8000) NULL
	, [log_message] [NVARCHAR](8000) NULL
	, [log_machine_name] [NVARCHAR](8000) NULL
	, [log_user_name] [NVARCHAR](8000) NULL
	, [log_call_site] [NVARCHAR](8000) NULL
	, [log_thread] [NVARCHAR](100) NULL
	, [log_exception] [NVARCHAR](8000) NULL
	, [log_stacktrace] [NVARCHAR](8000) NULL
	, CONSTRAINT [PK_system_logging] PRIMARY KEY CLUSTERED ([system_logging_guid] ASC)
	WITH (
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS  = ON
		) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[system_logging] ADD CONSTRAINT [DF_system_logging_system_logging_guid] DEFAULT (NEWID()) FOR [system_logging_guid]
GO
ALTER TABLE [dbo].[system_logging] ADD CONSTRAINT [DF_system_logging_entered_date] DEFAULT (GETDATE()) FOR [entered_date]
GO

-- for email
ALTER TRIGGER [dbo].[LogEmail] ON [dbo].[system_logging] AFTER INSERT
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from 
-- interfering with SELECT statements. 
SET NOCOUNT ON;

DECLARE @TO NVARCHAR(100)
DECLARE @TITLE NVARCHAR(100)
DECLARE @LOGMESSAGE NVARCHAR(8000)
DECLARE @LOGLEVEL NVARCHAR(100)
SET @TO = 'shockz@iquest.co.kr' 
SET @TITLE = 'Logging'
SET @LOGLEVEL = (SELECT [log_level] FROM [inserted]) 
SET @LOGMESSAGE = (SELECT
'User Date:' + CHAR(9) + CHAR(9) + [log_date] + CHAR(13) + CHAR(10) +
'Computer:'+ CHAR(9) + [log_machine_name] + CHAR(13) + CHAR(10) +
'User:' + CHAR(9) + CHAR(9) + [log_user_name] + CHAR(13) + CHAR(10) + 
'Level:' + CHAR(9)+ [log_level] + CHAR(13) + CHAR(10) +
'Logger:' + CHAR(9)+ [log_logger] + CHAR(13) + CHAR(10) +
'Thread:'+ CHAR(9) + [log_thread] + CHAR(13) + CHAR(10) +
'StackTrace:'+ CHAR(9) + [log_stacktrace] + CHAR(13) + CHAR(10) +
'CallSite:'+ CHAR(9) + [log_call_site] + CHAR(13) + CHAR(10) +
'Message:' + CHAR(9) + [log_message] + CHAR(13) + CHAR(10) +
'Exception:'+ CHAR(9) + [log_exception] AS 'emailmessage'
FROM [inserted])

IF @LOGLEVEL <> 'Info'
	EXEC msdb.dbo.sp_send_dbmail @recipients = @TO, @body =  @LOGMESSAGE, @subject = @TITLE, @profile_name = 'default'

END

-- DatabaseMailUserRole 지정
--EXEC msdb.dbo.sp_addrolemember @rolename = 'DatabaseMailUserRole', @membername = '사용자명';
--GO
