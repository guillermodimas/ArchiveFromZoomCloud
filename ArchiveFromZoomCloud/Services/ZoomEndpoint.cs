using ArchiveFromZoomCloud.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFromZoomCloud.Services
{
    public class ZoomEndpoint : IZoomEndpoint
    {
        private HttpClient _httpClient;
        private readonly ILogger<ZoomEndpoint> _logger;
        private readonly IConfiguration _config;
        public ZoomEndpoint(HttpClient httpClient, ILogger<ZoomEndpoint> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = configuration;
            InitializeClient();

        }
        private void InitializeClient()
        {
            string api = _config["ZoomDomain"]; //ConfigurationManager.AppSettings["api"]; //api URL from app.config
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(api);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public string GenerateJWTToken()
        {

            DateTime Expiry = DateTime.UtcNow.AddHours(6);
            string ApiKey = _config["ZoomAPIKeys:APIKey"];
            string ApiSecret = _config["ZoomAPIKeys:APISecret"];

            int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            // Create Security key  using private key above:
            // note that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecret));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Finally create a Token
            var header = new JwtHeader(credentials);

            //Zoom Required Payload
            var payload = new JwtPayload
            {
                { "iss", ApiKey},
                { "exp", ts },
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return handler.WriteToken(secToken);
        }
        public async Task SetAuthenticatedBearerToken()
        {
            string token = GenerateJWTToken();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            await Task.FromResult("");
        }

        public async Task<ZoomUsersModel> GetZoomUsers()
        {
            await SetAuthenticatedBearerToken();

            ZoomUsersModel users = new ZoomUsersModel();

            using (HttpResponseMessage response = await _httpClient.GetAsync("users?page_size=300"))
            {

                if (response.IsSuccessStatusCode)
                {
                    users = await response.Content.ReadFromJsonAsync<ZoomUsersModel>();
                    return users;
                }
                else
                {
                    return users;
                }
            }
        }
        public async Task<ZoomUserRecordingsModel> GetZoomUserRecordings(string UserID, string FromDate, string ToDate)
        {
            await SetAuthenticatedBearerToken();

            ZoomUserRecordingsModel usersRecordings = new ZoomUserRecordingsModel();

            using (HttpResponseMessage response = await _httpClient.GetAsync($"users/{UserID}/recordings?from={FromDate}&to={ToDate}&page_size=300"))
            {

                if (response.IsSuccessStatusCode)
                {
                    usersRecordings = await response.Content.ReadFromJsonAsync<ZoomUserRecordingsModel>();
                    return usersRecordings;
                }
                else
                {
                    return usersRecordings;
                }
            }
        }
        public async Task<HttpResponseMessage> DeleteRecordingFromZoom(string meetingID, string recordingID)
        {
            //https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/recordingdeleteone

            await SetAuthenticatedBearerToken();

            ZoomUserRecordingsModel usersRecordings = new ZoomUserRecordingsModel();

            using (HttpResponseMessage response = await _httpClient.DeleteAsync($"meetings/{meetingID}/recordings/{recordingID}?action=delete"))
            {

                return response;
            }
        }

    }
}
