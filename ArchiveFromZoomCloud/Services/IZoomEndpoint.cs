using ArchiveFromZoomCloud.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArchiveFromZoomCloud.Services
{
    public interface IZoomEndpoint
    {
        Task<HttpResponseMessage> DeleteRecordingFromZoom(string meetingID, string recordingID);
        string GenerateJWTToken();
        Task<ZoomUserRecordingsModel> GetZoomUserRecordings(string UserID, string FromDate, string ToDate);
        Task<ZoomUsersModel> GetZoomUsers();
        Task SetAuthenticatedBearerToken();
    }
}