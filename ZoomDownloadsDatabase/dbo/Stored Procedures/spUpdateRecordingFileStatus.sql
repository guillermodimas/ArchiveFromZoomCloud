



CREATE PROCEDURE [dbo].[spUpdateRecordingFileStatus]
	 @id varchar(100),
	 @FTPStatus varchar(50),
	 @FTPBackupDate datetime,
	 @BackupLocation varchar(max)
	
AS
begin
	set nocount on;
	
	BEGIN TRY
    BEGIN TRANSACTION
		
		UPDATE [dbo].[RecordingFiles]
		SET FTPBackupDate = @FTPBackupDate,
			FTPStatus = @FTPStatus,
			BackupLocation = @BackupLocation,
			[LastModifield] = GETDATE()
		Where [id] = @id
	   

    COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH


	

end