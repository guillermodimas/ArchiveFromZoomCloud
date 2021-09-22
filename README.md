# What is ArchiveFromZoomCloud?
Utility that leverages the Zoom API to comb through every cloud recording, save its META data into a database and then FTP those files to a dedicated location for archiving. Once the utility marks the files as archived, it once again leverages the Zoom API to remove it from the Zoom cloud to free up cloud storage. 
# Prerequisites

### Zoom API Keys

Before you can continue, you will need to create a Zoom API App that uses the JWT Credentials.
* For instructions on how to create and obtain your Zoom API app, visit https://marketplace.zoom.us/docs/guides/build
* In `appsettings.json`, insert the correspoding API Key and API Secret
![image](https://user-images.githubusercontent.com/5040055/134385067-e6971cb6-89ad-4bf2-b5af-876d4587d93e.png)
```json
{

  "ZoomAPIKeys": {
    "APIKey": "INSERT ZOOM APP API KEY HERE",
    "APISecret": "INSERT ZOOM APP API SECRET HERE"
  }
  
}
```

### SQL Server Configuration

Restore the SQL Server Database from the database project. 
Configure the SQL Server connection string in `appsettings.json`
```json
{

  "ConnectionStrings": {
    "ZoomDownloadsData": "INSERT SQL CONNECTION STRING HERE"
  }
  
}
```
# Useage

### Task #1 - Batch Zoom Metadata Copy
* In `Program.cs` you will see the three tasks that run when this app is launched. The first one  
```cs
await GetZoomUsersAndRecordingsFromAPI(); 
```
Is in charge of retreiving all Zoom users and populating corresponding META data to the SQL db. This will tell the following task what and where to find files to archive to the FTP.

---
*NOTE*
The Zoom API will not let you query recordings greater than 30 day date range, you will need to run this in 30 day increments as necessary to account for all your Zoom Cloud Recordings 
---
### Task #2 - Backup to FTP
* In `Program.cs` the second task is in charge of downloading the file to memory or local directory (depending on file size) and then uploading to the FTP location you have configured. This task will then mark the file FTPStatus as **Completed** so it doesn't get triggered to download the next time it is ran. 

```cs
BackupToFTP();
```
Insert your FTP directory and credentials in the corresponding are in `appsettings.json`

```json
{

  "FTPConfiguration": {
    "Directory": "ftp://22222222:21//FTP",
    "UserName": "",
    "Password": ""
  }
  
}
```
![image](https://user-images.githubusercontent.com/5040055/134404518-e212ade9-6d31-478b-94f6-69c892b394af.png)
![image](https://user-images.githubusercontent.com/5040055/134403455-6c507544-c113-4342-a2ac-f658ee142fde.png)

### Task #3 - Purge Zoom Cloud Recordings
* In `Program.cs` the last task handles the purging of Zoom cloud recordings after they have been successfully archived. 
```cs
await PurgeZoomRecordings();
```
This process will only take place **7** days after the archival date but can be changed in the `[dbo].[spLoadPurgableRecordings]` stored procedure
