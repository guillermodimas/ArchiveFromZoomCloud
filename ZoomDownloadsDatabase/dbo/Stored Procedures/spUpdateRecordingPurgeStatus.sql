




CREATE PROCEDURE [dbo].[spUpdateRecordingPurgeStatus]
	 @id varchar(100),
	 @purgeDate datetime,
	 @status varchar(2000)
	
	
AS
begin
	set nocount on;
	
	BEGIN TRY
    BEGIN TRANSACTION
		
		--IF(@status ='Error: Zoom API Response code (NotFound)') --file already deleted from Zoom Cloud prior to purging
		--BEGIN
		--	UPDATE [dbo].[RecordingFiles]
		--	SET [ZoomPurgedDate] = GETDATE(),
		--		[ZoomPurgeStatus] = 'ZoomDeleted',
		--		[LastModifield] = GETDATE()
		--	Where [id] = @id
		--END
		--ELSE
		BEGIN
			UPDATE [dbo].[RecordingFiles]
			SET [ZoomPurgedDate] = @purgeDate,
				[ZoomPurgeStatus] = @status,
				[LastModifield] = GETDATE()
			Where [id] = @id
		END
		
	   

    COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH


	

end
