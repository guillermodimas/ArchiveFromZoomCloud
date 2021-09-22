

CREATE PROCEDURE [dbo].[spInsertZoomUser]
	 @id varchar(50),
	 @first_name varchar(100),
	 @last_name varchar(100),
	 @email varchar(200),
	 @type int,
	 @pmi bigint,
	 @timezone varchar(300),
	 @verified int,
	 @dept varchar(1000),
	 @created_at datetime,
	 @last_login_time datetime,
	 @last_client_version varchar(200),
	 @language varchar(100),
	 @phone_number varchar(50),
	 @status varchar(100)


AS
begin
	set nocount on;
	
	BEGIN TRY
    BEGIN TRANSACTION
		
		INSERT INTO [dbo].[Users]
           ([id]
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
      ,[status])
     VALUES
           (@id, @first_name, @last_name, @email, @type, @pmi, @timezone, 
		   @verified, @dept, @created_at, @last_login_time, 
		   @last_client_version, @language, @phone_number, @status)
		
        COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH


	

end