using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

namespace AskDelphiBotAPIExample
{
    public class BotAPIConsumer
    {
        public static string LocalDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ConfigFilePath = Path.Combine(LocalDataDir, "bot-api-consumer-data.json");

        public BaSettings Settings { get; private set; }

        public BotAPIConsumer()
        {
            LoadConfiguration();
        }

        public void SetBasicData(string apiServer, string publicationGuid, string tenantGuid, string searchTopicGuid)
        {
            Settings.ApiServer = apiServer;
            Settings.PublicationGuid = publicationGuid;
            Settings.TenantGuid = tenantGuid;
            Settings.SearchTopicGuid = searchTopicGuid;

            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(Settings));
        }

        public void SetTokens(string jwtToken, string refreshToken)
        {
            Settings.JwtToken = jwtToken;
            Settings.RefreshToken = refreshToken;

            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(Settings));
        }

        public async Task<string> GetLoginUrl()
        {
            var client = CreateRestClient();
            var request = new RestRequest("TokenAPI/tokenurl", DataFormat.Json);
            request.AddQueryParameter("publicationGuid", Settings.PublicationGuid);
            IRestResponse response = client.Get(request);
            return await Task.FromResult(response.Content);
        }

        public async Task RefreshTokens()
        {
            var client = CreateRestClient();
            var request = new RestRequest("TokenAPI/refresh", DataFormat.Json);
            request.AddQueryParameter("publicationGuid", Settings.PublicationGuid);
            request.AddQueryParameter("token", Settings.JwtToken);
            request.AddQueryParameter("refreshToken", Settings.RefreshToken);
            DtoTokenRefreshResult response = await client.GetAsync<DtoTokenRefreshResult>(request);

            SetTokens(response.token, response.refresh);
        }

        public async Task<string> TestYourSearch()
        {
            var client = CreateRestClient();
            var request = new RestRequest("SearchAPI/search", DataFormat.Json);
            request.AddJsonBody(new DtoSearchParams
            {
                publicationGuid = Settings.PublicationGuid,
                searchTopicGuid = Settings.SearchTopicGuid,
                tenantGuid = Settings.TenantGuid,
                skip = 0,
                count = 25
            });
            request.AddHeader("Authorization", $"Bearer {Settings.JwtToken}");
            IRestResponse response = client.Post(request);
            return await Task.FromResult(response.Content);
        }

        public async Task<byte[]> DownloadAPIData(string outputFilename, DtoMapping mapping, string functionAPIServer = "https://kh-bot-api.azurewebsites.net/api")
        {
            var client = new RestClient(functionAPIServer);
            var request = new RestRequest($"GenerateExportDataFile/{WebUtility.UrlEncode(Settings.JwtToken)}/{WebUtility.UrlEncode(Settings.ApiServer)}/{WebUtility.UrlEncode(Settings.PublicationGuid)}/{WebUtility.UrlEncode(Settings.SearchTopicGuid)}/{WebUtility.UrlEncode(outputFilename)}", DataFormat.Json);
            request.AddJsonBody(mapping);
            request.Method = Method.POST;
            var data = client.DownloadData(request);
            Console.WriteLine($"{data.Length} bytes downloaded.");
            return await Task.FromResult(data);
        }


        private void LoadConfiguration()
        {
            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                Settings = JsonConvert.DeserializeObject<BaSettings>(json);
            }
            else
            {
                Settings = new BaSettings();
            }

        }

        public RestClient CreateRestClient()
        {
            if (Settings.ApiServer.StartsWith("https"))
            {
                return new RestClient($"{Settings.ApiServer}");
            }
            return new RestClient($"https://{Settings.ApiServer}.azurewebsites.net/api");
        }
    }
}
