CREATE TABLE [dbo].[Users] (
    [id]                  VARCHAR (50)   NOT NULL,
    [first_name]          VARCHAR (100)  NULL,
    [last_name]           VARCHAR (100)  NULL,
    [email]               VARCHAR (200)  NULL,
    [type]                INT            NULL,
    [pmi]                 BIGINT         NULL,
    [timezone]            VARCHAR (300)  NULL,
    [verified]            INT            NULL,
    [dept]                VARCHAR (1000) NULL,
    [created_at]          DATETIME       NULL,
    [last_login_time]     DATETIME       NULL,
    [last_client_version] VARCHAR (200)  NULL,
    [language]            VARCHAR (100)  NULL,
    [phone_number]        VARCHAR (50)   NULL,
    [status]              VARCHAR (100)  NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([id] ASC)
);

