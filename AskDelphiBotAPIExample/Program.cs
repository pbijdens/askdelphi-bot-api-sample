using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace AskDelphiBotAPIExample
{
    class Program
    {
        const string FunctionAPIServer = "https://kh-bot-api.azurewebsites.net/api";
        // const string FunctionAPIServer = "http://localhost:7071/api";

        static void Main(string[] args)
        {
            var task = new Task(async () =>
            {
                BotAPIConsumer botAPIConsumer = new BotAPIConsumer();

                Console.WriteLine($"Reading settings and tokens from: {BotAPIConsumer.ConfigFilePath} (delete this file to use different settings)");

                await EnsureSystemIsConfigured(botAPIConsumer);

                await DownloadSampleSeacrhResult(botAPIConsumer);
                await DownloadJsonBotDataExport(botAPIConsumer);
                await DownloadExcelBotDataExport(botAPIConsumer);

                Console.WriteLine("All done. Press enter to continue.");
            });
            task.Start();
            task.Wait();
            Console.ReadLine();
        }

        private static async Task DownloadSampleSeacrhResult(BotAPIConsumer botAPIConsumer)
        {
            string searchResult = await botAPIConsumer.TestYourSearch();
            string searchOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "bot-export-search-sample.json");
            File.WriteAllText(searchOutputPath, searchResult);
            Console.WriteLine($"Wrote raw search output to {searchOutputPath}");
        }

        private static async Task DownloadJsonBotDataExport(BotAPIConsumer botAPIConsumer)
        {
            DtoMapping mapping = CreateMapping(excel: false);
            byte[] downloadedData = await botAPIConsumer.DownloadAPIData("data.json", mapping, FunctionAPIServer);
            string jsonOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "bot-export-result.json");
            File.WriteAllText(jsonOutputPath, Encoding.UTF8.GetString(downloadedData));
            Console.WriteLine($"Wrote JSON output to {jsonOutputPath}");
        }

        private static async Task DownloadExcelBotDataExport(BotAPIConsumer botAPIConsumer)
        {
            DtoMapping mapping = CreateMapping(excel: true);
            byte[] downloadedData = await botAPIConsumer.DownloadAPIData("data.xlsx", mapping, FunctionAPIServer);
            string xlsxOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "bot-export-result.xlsx");
            File.WriteAllBytes(xlsxOutputPath, downloadedData);
            Console.WriteLine($"Wrote Excel output to {xlsxOutputPath}");
        }

        private static DtoMapping CreateMapping(bool excel = false)
        {
            return new DtoMapping
            {
                format = excel ? "excel" : "json",
                name = "root",
                columns = new System.Collections.Generic.List<DtoMappingColumn>() {
                        new DtoMappingColumn
                        {
                            name = "id",
                            mapping = "text", 
                            source = "id", // property fromt he search result to be used, possibly followed by ;xpath
                            xslt = null
                        },
                        new DtoMappingColumn
                        {
                            name = "title",
                            mapping = "text",
                            source = "title",
                            xslt = null
                        },
                        new DtoMappingColumn
                        {
                            name = "url",
                            mapping = "text",
                            source = "url",
                            xslt = null
                        },
                        new DtoMappingColumn
                        {
                            name = "bodyXML",
                            mapping = "raw",
                            source = "bodyXML",
                            xslt = null // could optionally provide an XSLT here that's applied to the bodyxml before taking the inner text, must set type to "xslt", otherwise this is ignored
                        },
                        new DtoMappingColumn
                        {
                            name = "firstParagraph",
                            mapping = "text",
                            source = "bodyXML;//p", // example of using a very simple xpath. when more nodes match, we take the first
                            xslt = null
                        },
                    }
            };
        }

        private static async Task EnsureSystemIsConfigured(BotAPIConsumer botAPIConsumer)
        {
            if (string.IsNullOrWhiteSpace(botAPIConsumer.Settings.PublicationGuid))
            {
                Console.WriteLine("Which server should be used (e.g. imola-staging-api-9b73460a)");
                string apiServer = Console.ReadLine();

                Console.WriteLine("Which tenant are we doing this for? (e.g. f63116fc-12dc-4069-a76e-4c78962e1535)?");
                string tenantGuid = Console.ReadLine();

                Console.WriteLine("Which publication should be used (e.g. 9e3ac79e-399d-4e19-89e5-e2e959379d87)?");
                string publicationGuid = Console.ReadLine();

                Console.WriteLine("Which search topic  should be used (e.g. c28807e7-7b81-4937-9d23-e683a363723c)?");
                string searchTopicGuid = Console.ReadLine();

                botAPIConsumer.SetBasicData(apiServer, publicationGuid, tenantGuid, searchTopicGuid);
            }

            if (string.IsNullOrWhiteSpace(botAPIConsumer.Settings.JwtToken))
            {
                string loginUrl = await botAPIConsumer.GetLoginUrl();
                Console.WriteLine($"Go to {loginUrl}, sign in then copy the reponse headers X-Authentication-Token and X-Authentication-RefreshToken here when requested.");

                Console.WriteLine("Enter the value of the X-Authentication-Token response header (JWT token):");
                string jwtToken = Console.ReadLine();

                Console.WriteLine("Enter the value of the X-Authentication-RefreshToken header (refresh token):");
                string refreshToken = Console.ReadLine();

                botAPIConsumer.SetTokens(jwtToken, refreshToken);
            }
            else
            {
                Console.WriteLine("Got a token pair from the settings file, not logging in again, refreshing the tokens.");
                await botAPIConsumer.RefreshTokens();
                Console.WriteLine("Tokens refreshed, result stored in settings.");
            }
        }
    }
}
