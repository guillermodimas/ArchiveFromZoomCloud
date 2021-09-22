


CREATE PROCEDURE [dbo].[spLoadAllZoomUsers]

AS
begin
	set nocount on;
	
	SELECT [id]
      ,[first_name]
      ,[last_name]
      ,[email]
      ,[type]
      ,[pmi]
      ,[timezone]
      ,[verified]
      ,[dept]
      ,[created_at]
      ,[last_login_time]
      ,[last_client_version]
      ,[language]
      ,[phone_number]
      ,[status]
  FROM [dbo].[Users]

end