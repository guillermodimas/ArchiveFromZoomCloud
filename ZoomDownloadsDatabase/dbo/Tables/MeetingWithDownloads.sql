CREATE TABLE [dbo].[MeetingWithDownloads] (
    [uuid]            VARCHAR (100) NOT NULL,
    [id]              VARCHAR (50)  NULL,
    [account_id]      VARCHAR (100) NULL,
    [host_id]         VARCHAR (50)  NULL,
    [topic]           VARCHAR (MAX) NULL,
    [type]            INT           NULL,
    [start_time]      DATETIME      NULL,
    [timezone]        VARCHAR (100) NULL,
    [duration]        INT           NULL,
    [total_size]      BIGINT        NULL,
    [recording_count] INT           NULL,
    [share_url]       VARCHAR (MAX) NULL,
    CONSTRAINT [PK_MeetingWithDownloads] PRIMARY KEY CLUSTERED ([uuid] ASC)
);

