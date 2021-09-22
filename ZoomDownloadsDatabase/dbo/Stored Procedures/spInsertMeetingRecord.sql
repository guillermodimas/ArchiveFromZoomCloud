
CREATE PROCEDURE [dbo].[spInsertMeetingRecord]
	 @uuid varchar(100),
	 @id varchar(50),
	 @account_id varchar(100),
	 @host_id varchar(50),
	 @topic varchar(50),
	 @type int,
	 @start_time datetime,
	 @timezone varchar(100),
	 @duration int,
	 @total_size bigint,
	 @recording_count int,
	 @share_url varchar(max)

AS
begin
	set nocount on;
	
	BEGIN TRY
    BEGIN TRANSACTION
		
		INSERT INTO [dbo].[MeetingWithDownloads]
           ([uuid]
           ,[id]
           ,[account_id]
           ,[host_id]
           ,[topic]
           ,[type]
           ,[start_time]
           ,[timezone]
           ,[duration]
           ,[total_size]
           ,[recording_count]
           ,[share_url])
     VALUES
           (@uuid,@id,@account_id,@host_id,@topic,@type,@start_time,@timezone,@duration,@total_size,@recording_count,@share_url)
		
        COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH


	

end