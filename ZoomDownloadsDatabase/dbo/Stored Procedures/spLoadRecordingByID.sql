


CREATE PROCEDURE [dbo].[spLoadRecordingByID]
@id varchar(100)
AS
begin
	set nocount on;
	
	SELECT  [id]
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
      ,[LastModifield]
  FROM [dbo].[RecordingFiles]
  WHERE id = @id
 

end