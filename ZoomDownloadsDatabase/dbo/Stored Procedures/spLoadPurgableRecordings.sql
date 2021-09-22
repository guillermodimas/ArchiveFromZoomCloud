

CREATE PROCEDURE [dbo].[spLoadPurgableRecordings]

AS
begin
	set nocount on;
	
	
	SELECT [id]
      ,meeting_id = (Select id FROM [dbo].[MeetingWithDownloads] where uuid = [dbo].[RecordingFiles].meeting_id)
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
	  --,[ZoomPurgeStatus]
      ,[BackupLocation]
      ,[FTPStatus]
	  ,LastModifield
  FROM [dbo].[RecordingFiles]
  WHERE [FTPBackupDate] is not null and [ZoomPurgedDate] is null
  AND [recording_start] < dateadd(week,-1,getdate()) --PURGE RECORDINGS FROM ZOOM THAT ARE OVER A WEEK OLD
  ORDER BY recording_start 

end