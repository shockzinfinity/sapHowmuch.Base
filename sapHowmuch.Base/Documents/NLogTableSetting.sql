CREATE TABLE [dbo].[system_logging] (
	[system_logging_guid] [uniqueidentifier] ROWGUIDCOL NOT NULL
	, [entered_date] [datetime] NULL
	, [log_application] [varchar](200) NULL
	, [log_date] [varchar](100) NULL
	, [log_level] [varchar](100) NULL
	, [log_logger] [varchar](8000) NULL
	, [log_message] [varchar](8000) NULL
	, [log_machine_name] [varchar](8000) NULL
	, [log_user_name] [varchar](8000) NULL
	, [log_call_site] [varchar](8000) NULL
	, [log_thread] [varchar](100) NULL
	, [log_exception] [varchar](8000) NULL
	, [log_stacktrace] [varchar](8000) NULL
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


-- 이메일 처리를 위한 부분
ALTER TRIGGER [dbo].[LogEmail] ON [dbo].[system_logging] AFTER INSERT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements. 
SET NOCOUNT ON;

DECLARE @ToEmail varchar(100)
DECLARE @Title varchar(100)
DECLARE @logmessage varchar(8000)
DECLARE @loglevel as varchar(100)
SET @ToEmail = 'shockz@iquest.co.kr' 
SET @Title = 'Error'
SET @loglevel = (SELECT log_level FROM inserted) 
SET @logmessage = (SELECT
'User Date:' + CHAR(9) + CHAR(9) + log_date + CHAR(13) + CHAR(10) +
'Computer:'+ CHAR(9) + log_machine_name + CHAR(13) + CHAR(10) +
'User:' + CHAR(9) + CHAR(9) + log_user_name + CHAR(13) + CHAR(10) + 
'Level:' + CHAR(9)+ log_level + CHAR(13) + CHAR(10) +
'Logger:' + CHAR(9)+ log_logger + CHAR(13) + CHAR(10) +
'Thread:'+ CHAR(9) + log_thread + CHAR(13) + CHAR(10) +
'StackTrace:'+ CHAR(9) + log_stacktrace + CHAR(13) + CHAR(10) +
'CallSite:'+ CHAR(9) + log_call_site + CHAR(13) + CHAR(10) +
'Message:' + CHAR(9) + log_message + CHAR(13) + CHAR(10) +
'Exception:'+ CHAR(9) + log_exception AS 'emailmessage'
FROM inserted)

IF @loglevel <>'Info' 
	EXEC msdb.dbo.sp_send_dbmail @recipients=@ToEmail, @body= @logmessage, @subject = @Title, @profile_name = 'default'

END
