using ArchiveFromZoomCloud.Models;
using System;
using System.Collections.Generic;

namespace ArchiveFromZoomCloud.DataAccess
{
    public interface IZoomAPIData
    {
        void InsertMeetingRecord(ZoomUserRecordingsModel.Meeting zoomUserRecordingsMeetings);
        void InsertRecordingFile(ZoomUserRecordingsModel.Recording_Files recording_Files);
        void InsertZoomUser(ZoomUsersModel.User user);
        ZoomUserRecordingsModel.Meeting LoadMeetingByUUID(string uuid);
        List<ZoomUserRecordingsModel.Meeting> LoadMeetingRecords();
        List<RecordingFilesModel> LoadPendingRecordings();
        List<RecordingFilesModel> LoadPurgableRecordings();
        ZoomUserRecordingsModel.Recording_Files LoadRecordingByID(string id);
        List<ZoomUsersModel.User> LoadZoomUsers();
        void UpdateRecordingFileStatus(string id, string FTPStatus, DateTime? FTPBackupDate, string BackupLocation);
        void UpdateRecordingPurgeStatus(string id, DateTime? purgeDate, string status);
    }
}