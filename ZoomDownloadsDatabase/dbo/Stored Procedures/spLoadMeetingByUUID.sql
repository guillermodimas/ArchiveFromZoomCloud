


CREATE PROCEDURE [dbo].[spLoadMeetingByUUID]
@uuid varchar(100)
AS
begin
	set nocount on;
	
	SELECT  
	   [uuid]
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
      ,[share_url]
  FROM [dbo].[MeetingWithDownloads]
  WHERE uuid = @uuid
 

end