using ArchiveFromZoomCloud.DataAccess;
using ArchiveFromZoomCloud.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
namespace ArchiveFromZoomCloud
{
    class Program
    {
        public static IConfiguration _Configuration;
        private static IZoomEndpoint _ZoomAPI;
        private static IZoomAPIData _ZoomData;
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            var host = Host.CreateDefaultBuilder()
             
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ISQLDataAccess, SQLDataAccess>();
                    services.AddTransient<IZoomAPIData, ZoomAPIData>();
                    services.AddScoped<ZoomEndpoint, ZoomEndpoint>();
                    services.AddScoped(sp => new HttpClient());
                }).Build();

            //instantiate SQL data access service 
            _ZoomData = ActivatorUtilities.CreateInstance<ZoomAPIData>(host.Services);

            //instantiate Zoom API calls service 
            _ZoomAPI = ActivatorUtilities.CreateInstance<ZoomEndpoint>(host.Services);

            await GetZoomUsersAndRecordingsFromAPI();
            BackupToFTP();
            await PurgeZoomRecordings();
        }
        static void BuildConfig(IConfigurationBuilder builder)
        {
            var name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            _Configuration = builder.Build();
        }
        public static async Task GetZoomUsersAndRecordingsFromAPI()
        {
            var usersModel = await _ZoomAPI.GetZoomUsers();
            if (usersModel != null)
            {
                // Get List of DB Users, only add if doesnt already exist
                var dbUsers = _ZoomData.LoadZoomUsers();

                foreach (var user in usersModel.users)
                {
                    // only add user to DB if doesnt already exist
                    var existingUser = dbUsers.Find(x => x.id == user.id);
                    if (existingUser == null)
                    {
                        //ADD NEW USER TO DB
                        _ZoomData.InsertZoomUser(user);
                    }



                    var UserRecordings = await _ZoomAPI.GetZoomUserRecordings(user.id, DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
                    if (UserRecordings != null)
                    {
                        if (UserRecordings.total_records != 0)
                        {
                            foreach (var meeting in UserRecordings.meetings)
                            {
                                //only add Meeting to DB if doesnt already exist
                                var dbMeeting = _ZoomData.LoadMeetingByUUID(meeting.uuid);
                                if (dbMeeting == null)
                                {
                                    //Add New Meeting info to DB
                                    _ZoomData.InsertMeetingRecord(meeting);
                                }

                                if (meeting.recording_files != null)
                                {
                                    foreach (var recording in meeting.recording_files)
                                    {
                                        //only add Recording to DB if doesnt already exist
                                        var dbRecording = _ZoomData.LoadRecordingByID(recording.id);
                                        if (dbRecording == null)
                                        {
                                            if (recording.status == "completed") //only insert completed cloud recordings
                                            {
                                                //Add New recording info to DB
                                                _ZoomData.InsertRecordingFile(recording);
                                            }

                                        }
                                        else //if exists, make sure zoompurge date is NULL as file is still in the zoom cloud
                                        {
                                            _ZoomData.UpdateRecordingPurgeStatus(recording.id, null, null);
                                        }


                                    }
                                }

                            }
                        }

                    }
                }
            }
        }
        public static void BackupToFTP()
        {
            string accessToken = _ZoomAPI.GenerateJWTToken();
            var list = _ZoomData.LoadPendingRecordings();

            foreach (var record in list)
            {
                _ZoomData.UpdateRecordingFileStatus(record.id, "Starting", null, null);
                string downloadURL = record.download_url;
                string fileFormat = record.file_type;
                string location = DateTime.Now.ToString("MM-dd-yyyy"); //record.meeting_id; //meeting ID
                string fileName = $"{record.id}.{fileFormat}";

                string directory = _Configuration["FTPConfiguration:Directory"];
                
                Console.WriteLine($"Starting Download for file {fileName}");


                if (record.file_size >= 2147483648) //
                {
                    Console.WriteLine($"LARGE FILE process and FTP");
                    //if file larger than 2GB user this version, writes to temp and uploads to FTP, then deletes file
                    DateTime startTime = DateTime.UtcNow;
                    WebRequest request = WebRequest.Create($"{downloadURL}?access_token={accessToken}");
                    WebResponse response = request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (Stream fileStream = File.OpenWrite($@"{fileName}")) //write to local file in project folder
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead = responseStream.Read(buffer, 0, 4096);
                            while (bytesRead > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                                DateTime nowTime = DateTime.UtcNow;
                                if ((nowTime - startTime).TotalMinutes > 5)
                                {
                                    throw new ApplicationException(
                                        "Download timed out");
                                }
                                bytesRead = responseStream.Read(buffer, 0, 4096);
                            }
                        }
                    }

                    Console.WriteLine($"Completed Download for file {fileName}");

                    Console.WriteLine($"Starting LARGE File FTP Upload for file {fileName} to {directory}/{location}/{fileName}");

                    try
                    {
                        WebRequest requestftp = WebRequest.Create(directory + "/" + location);
                        requestftp.Method = WebRequestMethods.Ftp.MakeDirectory;
                        requestftp.Credentials = new NetworkCredential(_Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                        FtpWebResponse responseftp = (FtpWebResponse)requestftp.GetResponse();
                    }
                    catch (WebException ex)
                    {

                    }
                    //FTP Local File
                    using (var ftpClient = new WebClient())
                    {
                        try
                        {

                            ftpClient.Credentials = new NetworkCredential(_Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                            ftpClient.UploadFile(directory + "/" + location + "/" + fileName, WebRequestMethods.Ftp.UploadFile, $@"{fileName}");

                            var result = CheckFTPFileExists(directory + "/" + location + "/" + fileName, _Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                            //Delete local File
                            File.Delete($@"{fileName}");

                            if (result)
                            {
                                _ZoomData.UpdateRecordingFileStatus(record.id, "Completed", DateTime.Now, directory + "/" + location + "/" + fileName);
                                Console.WriteLine($"Completed FTP Upload for file {fileName}");
                            }
                            else
                            {
                                
                                Console.WriteLine($"Zoom Cloud Storage Backup Failed", $"Could not verify file uploaded for file: {fileName}");
                                _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, $"Could not verify file uploaded for file: {fileName}");
                            }
                        }
                        catch (WebException we)
                        {
                            FtpWebResponse responseError = (System.Net.FtpWebResponse)we.Response;
                            if (responseError != null)
                            {
                                if (responseError.StatusCode == FtpStatusCode.ActionNotTakenInsufficientSpace)
                                {
                                    
                                    Console.WriteLine($"Insufficient storage int FTP for file {fileName} to {directory}/{fileName} : {we.Message}");
                                    _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.Message);
                                }
                                else
                                {
                                    
                                    Console.WriteLine($"Error in FTP Upload for file {fileName} to {directory}/{fileName} : {we.Message}");
                                    _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.Message);
                                }
                            }
                            else
                            {
                                
                                Console.WriteLine($"Error in FTP Upload for file {fileName} to {directory}/{fileName} : {we.ToString()}");
                                _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.ToString());
                            }
                        }

                    }


                }
                else //else use regulare memory stream to download/upload to FTP
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            MemoryStream pdfMemoryStream = new MemoryStream(client.DownloadData($"{downloadURL}?access_token={accessToken}"));


                            Console.WriteLine($"Completed Download for file {fileName}");
                            Console.WriteLine($"Starting FTP Upload for file {fileName} to {directory}/{location}/{fileName}");

                            try
                            {
                                WebRequest request = WebRequest.Create(directory + "/" + location);
                                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                                request.Credentials = new NetworkCredential(_Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                            }
                            catch (WebException ex)
                            {

                            }


                            FtpUploadString(pdfMemoryStream, directory + "/" + location + "/" + fileName, _Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                            var exists = CheckFTPFileExists(directory + "/" + location + "/" + fileName, _Configuration["FTPConfiguration:UserName"], _Configuration["FTPConfiguration:Password"]);
                            if (exists)
                            {
                                _ZoomData.UpdateRecordingFileStatus(record.id, "Completed", DateTime.Now, directory + "/" + location + "/" + fileName);
                                Console.WriteLine($"Completed FTP Upload for file {fileName}");
                            }
                            else
                            {
                                
                                Console.WriteLine($"Zoom Cloud Storage Backup Failed", $"Could not verify file uploaded for file: {fileName}");
                                _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, $"Could not verify file uploaded for file: {fileName}");
                            }


                        }
                    }
                    catch (WebException we)
                    {
                        FtpWebResponse responseError = (System.Net.FtpWebResponse)we.Response;
                        if (responseError != null)
                        {
                            if (responseError.StatusCode == FtpStatusCode.ActionNotTakenInsufficientSpace)
                            {
                                
                                Console.WriteLine($"Insufficient storage int FTP for file {fileName} to {directory}/{fileName} : {we.Message}");
                                _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.Message);
                            }
                            else
                            {
                               
                                Console.WriteLine($"Error in FTP Upload for file {fileName} to {directory}/{fileName} : {we.Message}");
                                _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.Message);
                            }
                        }
                        else
                        {
                            
                            Console.WriteLine($"Error in FTP Upload for file {fileName} to {directory}/{fileName} : {we.Message}");
                            _ZoomData.UpdateRecordingFileStatus(record.id, "ERROR", null, we.Message);
                        }
                    }


                }





            }
        }
        public static async Task PurgeZoomRecordings()
        {
            var recordingsToPurge = _ZoomData.LoadPurgableRecordings();
            if (recordingsToPurge != null)
            {
                foreach (var recording in recordingsToPurge)
                {
                    //delete backuped recording from Zoom Cloud Storage
                    var response = await _ZoomAPI.DeleteRecordingFromZoom(recording.meeting_id, recording.id);
                    if (response.IsSuccessStatusCode)
                    {
                        //Update recording status on DB
                        _ZoomData.UpdateRecordingPurgeStatus(recording.id, DateTime.Now, "Completed");
                    }
                    else
                    {
                        //update error for purge status
                        _ZoomData.UpdateRecordingPurgeStatus(recording.id, null, $"Error: Zoom API Response code ({response.StatusCode})");
                    }



                }
            }
        }
        private static string FtpUploadString(MemoryStream memStream, string to_uri, string user_name, string password)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(to_uri);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials =
                new NetworkCredential(user_name, password);
            request.UseBinary = true;
            byte[] buffer = new byte[memStream.Length];
            memStream.Read(buffer, 0, buffer.Length);
            memStream.Close();
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(buffer, 0, buffer.Length);
            }
            return string.Empty;
        }
        private static bool CheckFTPFileExists(string to_uri, string user_name, string password)
        {

            WebRequest request = WebRequest.Create(to_uri);
            request.Credentials = new NetworkCredential(user_name, password);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            try
            {
                request.GetResponse();
                return true;
            }
            catch (WebException e)
            {
                return false;
            }
        }
    }
}
