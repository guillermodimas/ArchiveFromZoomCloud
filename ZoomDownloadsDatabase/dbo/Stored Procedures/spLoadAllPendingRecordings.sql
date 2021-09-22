

CREATE PROCEDURE [dbo].[spLoadAllPendingRecordings]

AS
begin
	set nocount on;
	
	SELECT [id]
      ,[meeting_id]
      ,[recording_start]
      ,[recording_end]
      ,[file_type]
      ,[file_size]
      ,[play_url]
      ,[download_url]
      ,[status]
      ,[recording_type]
      ,[FTPBackupDate]
      ,[ZoomPurgedDate]
      ,[BackupLocation]
      ,[FTPStatus]
	  ,LastModifield
  FROM [dbo].[RecordingFiles]
  WHERE [FTPBackupDate] is null
 -- AND meeting_id = 'Z1XLicaoT/i1gBH6oYT7Pg=='
  ORDER BY [recording_start] 

end