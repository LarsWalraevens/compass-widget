using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Text;
using Vvoids.Api.Base;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions;

public class TestingLinux(ILogger<TestingLinux> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public string[] Commands { get; set; }
    }

    [Function(nameof(TestingLinux))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/testing/linux")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);

        //"ls | grep \"2024\"" filters on only files with 2024 in it
        string[] listFileNamesInFolderCommandSequence = new string[] { "cd RSYNC", "cd Upload", "cd {FOLDER}", "ls | grep \"{YEAR}\" | grep \"{MONTH}\" | grep \"{DAY}\"" };
        string[] readFileFromFolderCommandSequence = new string[] { "cd RSYNC", "cd Upload", "cd {FOLDER}", "cat {FILE}" };
        string[] readFileSizeFromFolderCommandSequence = new string[] { "cd RSYNC", "cd Upload", "cd {FOLDER}", "du -sh {FILE}" };
        List<string> currentCommandSequence = new List<string>();

        (string folder, int providerId, int sourceId)[] dataStructures = new (string folder, int providerId, int sourceId)[] { ("GPS_DATA", 8, 5), ("NMEA_SER_DATA", 8, 6), ("NMEA_TCP_DATA", 8, 6) };

        using (SshClient client = new SshClient("77.73.99.197", 6122, "1680", "Vessel@Secure!"))
        {
            try
            {
                client.Connect();

                if (client.IsConnected)
                {
                    props.Services.Log.Info($"Linux Connected");

                    props.Services.Log.Info($"Folders to search: {dataStructures.Length}");

                    foreach ((string folder, int providerId, int sourceId) in dataStructures)
                    {
                        currentCommandSequence = listFileNamesInFolderCommandSequence.Select(x => x.Replace("{FOLDER}", folder).Replace("{YEAR}", DateTime.UtcNow.Year.ToString()).Replace("{MONTH}", DateTime.UtcNow.ToString("MM")).Replace("{DAY}", DateTime.UtcNow.ToString("dd"))).ToList();
                        string[] files = client.CreateCommand(string.Join(" ; ", currentCommandSequence)).Execute().Split("\n", StringSplitOptions.RemoveEmptyEntries);

                        props.Services.Log.Info($"Files in {folder}: {files.Length}");

                        foreach (string file in files)
                        {
                            bool useReport = true;

                            StringBuilder report = new StringBuilder();
                            report = report.AppendLine($"{file}");

                            if (useReport)
                            {
                                bool showReport = false;

                                currentCommandSequence = readFileFromFolderCommandSequence.Select(x => x.Replace("{FOLDER}", folder).Replace("{FILE}", file)).ToList();
                                string[] content = client.CreateCommand(string.Join(" ; ", currentCommandSequence)).Execute().Split("\n", StringSplitOptions.RemoveEmptyEntries);

                                foreach (string nmeaLine in content)
                                {
                                    if (new string[] { "$GPRMC" }.Any(nmeaLine.StartsWith))
                                    {
                                        showReport = true;
                                        report = report.AppendLine($"{nmeaLine}\n");
                                    }
                                }

                                if (showReport)
                                {
                                    props.Services.Log.Info($"{report.ToString()}\n\n");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Could not connect to the SSH server.");
                }
            }
            catch (Exception ex)
            {
                props.Services.Log.Error($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Disconnect from the server
                client.Disconnect();
            }
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}