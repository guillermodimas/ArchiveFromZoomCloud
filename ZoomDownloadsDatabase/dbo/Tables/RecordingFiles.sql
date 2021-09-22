CREATE TABLE [dbo].[RecordingFiles] (
    [id]              VARCHAR (100)  NOT NULL,
    [meeting_id]      VARCHAR (50)   NULL,
    [recording_start] DATETIME       NULL,
    [recording_end]   DATETIME       NULL,
    [file_type]       VARCHAR (50)   NULL,
    [file_size]       BIGINT         NULL,
    [play_url]        VARCHAR (MAX)  NULL,
    [download_url]    VARCHAR (MAX)  NULL,
    [status]          VARCHAR (100)  NULL,
    [recording_type]  VARCHAR (100)  NULL,
    [FTPBackupDate]   DATETIME       NULL,
    [FTPStatus]       VARCHAR (50)   NULL,
    [BackupLocation]  VARCHAR (MAX)  NULL,
    [ZoomPurgedDate]  DATETIME       NULL,
    [ZoomPurgeStatus] VARCHAR (2000) NULL,
    [LastModifield]   DATETIME       NULL,
    CONSTRAINT [PK_RecordingFiles] PRIMARY KEY CLUSTERED ([id] ASC)
);

