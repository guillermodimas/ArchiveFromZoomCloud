using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFromZoomCloud.Models
{
    public class RecordingFilesModel
    {
        public string id { get; set; }
        public string meeting_id { get; set; }
        public DateTime recording_start { get; set; }
        public DateTime recording_end { get; set; }
        public string file_type { get; set; }
        public long file_size { get; set; }
        public string play_url { get; set; }
        public string download_url { get; set; }
        public string status { get; set; }
        public string recording_type { get; set; }
        public DateTime? FTPBackupDate { get; set; }
        public DateTime? ZoomPurgedDate { get; set; }
        public string BackupLocation { get; set; }
        public string FTPStatus { get; set; }
        public string LastModifieldUTC { get; set; }

    }
}
