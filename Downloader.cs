using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace OsuBeatmapDownloader;

public class Downloader
{
    static string url = "https://osu.ppy.sh/";

    public static void DownloadMap(string mapId, string accessToken)
    {
        FileStore<string> archive = new FileStore<string>("archive/archive_beatmapset.json");
        if (archive.Items.Contains(mapId))
        {
            Console.WriteLine(mapId + " skipped, already downloaded");
            return;
        }

        string endpoint = url + "api/v2/beatmapsets/" + mapId + "/download?noVideo=1";
        string outfile = "output/" + mapId + ".osz";

        FileUtils.PreparePath(outfile, true, false);

        bool isCompleted = false;
        Exception error = null;

        WebClient webClient = new WebClient();
        webClient.Headers.Add("Authorization", "Bearer " + accessToken);


        ProgressBar progressBar = new ProgressBar();

        webClient.DownloadProgressChanged +=
            ((sender, args) => progressBar.Report((double) args.ProgressPercentage / 100));

        webClient.DownloadFileCompleted += (sender, args) =>
        {
            error = args.Error;
            isCompleted = true;
        };
        webClient.DownloadFileAsync(new Uri(endpoint), outfile);

        while (!isCompleted)
        {
            Thread.Sleep(1000);
        }

        if (error != null)
        {
            Console.WriteLine("Error occured when beatmap downloaded: " + error.Message);
            Console.WriteLine(endpoint);
            File.Delete(outfile);
        }
        else
        {
            archive.Add(mapId);
            archive.Store();
        }

        progressBar.Dispose();
    }
}