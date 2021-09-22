
CREATE PROCEDURE [dbo].[spInsertRecordingFile]
	 @id varchar(100),
	 @meeting_id varchar(50),
	 @recording_start datetime,
	 @recording_end datetime,
	 @file_type varchar(50),
	 @file_size bigint,
	 @play_url varchar(max),
	 @download_url varchar(max),
	 @status varchar(100),
	 @recording_type varchar(100)
	
AS
begin
	set nocount on;
	
	BEGIN TRY
    BEGIN TRANSACTION
		
	INSERT INTO [dbo].[RecordingFiles]
	(		[id]
           ,[meeting_id]
           ,[recording_start]
           ,[recording_end]
           ,[file_type]
           ,[file_size]
           ,[play_url]
           ,[download_url]
           ,[status]
           ,[recording_type]
		   ,LastModifield
           )
     VALUES
           (@id,@meeting_id,@recording_start,@recording_end,@file_type, @file_size,@play_url, @download_url, @status,@recording_type, GETDATE())
		
        COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH


	

end