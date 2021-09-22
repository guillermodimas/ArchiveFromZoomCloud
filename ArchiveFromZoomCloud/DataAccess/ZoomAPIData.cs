using ArchiveFromZoomCloud.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFromZoomCloud.DataAccess
{
    public class ZoomAPIData : IZoomAPIData
    {
        private readonly ISQLDataAccess _sQLDataAccess;
        private readonly ILogger<ZoomAPIData> _logger;

        public ZoomAPIData(ISQLDataAccess sQLDataAccess, ILogger<ZoomAPIData> logger)
        {
            _sQLDataAccess = sQLDataAccess;
            _logger = logger;

        }
        public List<RecordingFilesModel> LoadPendingRecordings()
        {
            try
            {
                return _sQLDataAccess.LoadData<RecordingFilesModel, dynamic>("dbo.spLoadAllPendingRecordings", new { }, "ZoomDownloadsData"); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public List<RecordingFilesModel> LoadPurgableRecordings()
        {
            try
            {
                return _sQLDataAccess.LoadData<RecordingFilesModel, dynamic>("dbo.spLoadPurgableRecordings", new { }, "ZoomDownloadsData"); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public List<ZoomUsersModel.User> LoadZoomUsers()
        {
            try
            {
                return _sQLDataAccess.LoadData<ZoomUsersModel.User, dynamic>("dbo.spLoadAllZoomUsers", new { }, "ZoomDownloadsData"); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public void InsertZoomUser(ZoomUsersModel.User user)
        {
            try
            {
                _sQLDataAccess.SaveData("dbo.spInsertZoomUser", user, "ZoomDownloadsData"); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public ZoomUserRecordingsModel.Meeting LoadMeetingByUUID(string uuid)
        {
            try
            {
                return _sQLDataAccess.LoadData<ZoomUserRecordingsModel.Meeting, dynamic>("dbo.spLoadMeetingByUUID", new { uuid }, "ZoomDownloadsData").FirstOrDefault(); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public List<ZoomUserRecordingsModel.Meeting> LoadMeetingRecords()
        {
            try
            {
                return _sQLDataAccess.LoadData<ZoomUserRecordingsModel.Meeting, dynamic>("dbo.spLoadMeetingRecords", new { }, "ZoomDownloadsData"); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public void UpdateRecordingFileStatus(string id, string FTPStatus, DateTime? FTPBackupDate, string BackupLocation)
        {
            try
            {
                _sQLDataAccess.SaveData("dbo.spUpdateRecordingFileStatus", new { id, FTPStatus, FTPBackupDate, BackupLocation }, "ZoomDownloadsData");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
        public void UpdateRecordingPurgeStatus(string id, DateTime? purgeDate, string status)
        {
            try
            {
                _sQLDataAccess.SaveData("dbo.spUpdateRecordingPurgeStatus", new { id, purgeDate, status }, "ZoomDownloadsData");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
        public void InsertMeetingRecord(ZoomUserRecordingsModel.Meeting zoomUserRecordingsMeetings)
        {
            try
            {
                _sQLDataAccess.SaveData("dbo.spInsertMeetingRecord", new
                {
                    zoomUserRecordingsMeetings.uuid,
                    id = zoomUserRecordingsMeetings.id.ToString(),
                    zoomUserRecordingsMeetings.account_id,
                    zoomUserRecordingsMeetings.host_id,
                    zoomUserRecordingsMeetings.topic,
                    zoomUserRecordingsMeetings.type,
                    zoomUserRecordingsMeetings.start_time,
                    zoomUserRecordingsMeetings.timezone,
                    zoomUserRecordingsMeetings.duration,
                    zoomUserRecordingsMeetings.total_size,
                    zoomUserRecordingsMeetings.recording_count,
                    zoomUserRecordingsMeetings.share_url
                }, "ZoomDownloadsData");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
        public ZoomUserRecordingsModel.Recording_Files LoadRecordingByID(string id)
        {
            try
            {
                return _sQLDataAccess.LoadData<ZoomUserRecordingsModel.Recording_Files, dynamic>("dbo.spLoadRecordingByID", new { id }, "ZoomDownloadsData").FirstOrDefault(); //no parameters needed (selecting all)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public void InsertRecordingFile(ZoomUserRecordingsModel.Recording_Files recording_Files)
        {
            try
            {
                DateTime tempStart;
                if (DateTime.TryParse(recording_Files.recording_start, out tempStart))
                {

                }
                else
                {
                    recording_Files.recording_start = "1/1/1900";
                }
                DateTime tempEnd;
                if (DateTime.TryParse(recording_Files.recording_end, out tempEnd))
                {

                }
                else
                {
                    recording_Files.recording_end = "1/1/1900";
                }
                _sQLDataAccess.SaveData("dbo.spInsertRecordingFile", recording_Files, "ZoomDownloadsData");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        //spUpdateRecordingFileStatus
    }
}
