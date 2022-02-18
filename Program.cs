// See https://aka.ms/new-console-template for more information


using System.Net.Http.Headers;
using System.Text;
using OsuBeatmapDownloader;

class Program
{
    static void Main(string[] args)
    {
        string username = null, password = null, input = null;
        bool convertIdToSet = false;
        List<string> ids = new List<string>();
        foreach (var s in args)
        {
            if (s.Contains("-login="))
            {
                username = s.Split("=")[1];
            }

            if (s.Contains("-password="))
            {
                password = s.Split("=")[1];
            }

            if (s.Contains("-input="))
            {
                input = s.Split("=")[1];
            }

            if (s.Contains("-ids="))
            {
                ids.AddRange(s.Split("=")[1].Split(","));
            }

            if (s.Contains("-convert"))
            {
                convertIdToSet = true;
            }

            if (s.Contains("-help"))
            {
                Console.WriteLine(
                    "Usage: -login=user -password=123 -convert -input=path/to/file.json -ids=1234,1234,4231 \n// Input file must be json array of strings \n// -convert param converts beatmap_id to beatmap_set via api");
                Environment.Exit(1);
            }
        }

        if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
        {
            Console.WriteLine("You need pass credentials. Try -help");
            Environment.Exit(1);
        }

        if (String.IsNullOrEmpty(input) && ids.Count == 0)
        {
            Console.WriteLine("You need pass beatmaps ids. Try -help");
            Environment.Exit(1);
        }

        if (!String.IsNullOrEmpty(input))
        {
            FileStore<string> inputStore = new FileStore<string>(input, false);
            ids.AddRange(inputStore.Items);
        }

        ids = ids.Distinct().ToList();

        string accessToken = OAuth.GetAccessToken(username, password);

        Console.WriteLine("User " + username + " loaded");

        if (convertIdToSet)
        {
            int idsLength = ids.Count;
            ids = Converter.BatchIdToSet(ids, accessToken);
            int setLength = ids.Count;
            Console.WriteLine(idsLength + " beatmap_ids converted to " + setLength + " beatmap_sets");
        }

        Console.WriteLine("Downloading " + ids.Count + " beatmaps");

        int index = 1;
        foreach (var id in ids)
        {
            Console.WriteLine("Downloading " + index + " #" + id + " beatmap");
            Downloader.DownloadMap(id, accessToken);
            index++;
        }
    }
}