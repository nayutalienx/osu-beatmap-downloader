using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace OsuBeatmapDownloader;

public class Converter
{
    static string url = "https://osu.ppy.sh/";

    class Beatmaps
    {
        public Beatmap[] beatmaps { get; set; }
    }

    class Beatmap
    {
        public int id { get; set; }
        public int beatmapset_id { get; set; }
    }

    class IdSet
    {
        public string id { get; set; }
        public string set { get; set; }
    }

    public static List<string> BatchIdToSet(List<string> ids, string accessToken)
    {
        FileStore<IdSet> archive = new FileStore<IdSet>("archive/archive_convert.json");

        List<string> notCached = new List<string>();

        bool cachePrinted = false;

        foreach (var id in ids)
        {
            bool found = false;
            foreach (var archiveItem in archive.Items)
            {
                if (id.Equals(archiveItem.id))
                {
                    found = true;
                    if (!cachePrinted)
                    {
                        Console.WriteLine("Some ids found in store. Cache will be used for convertion to beatmap_set");
                        cachePrinted = true;
                    }

                    break;
                }
            }

            if (!found)
            {
                notCached.Add(id);
            }
        }


        foreach (List<string> list in SplitList(notCached, 10))
        {
            Console.WriteLine("Converting " + list.Count + " beatmap ids via OSU API");
            string p = Strings.Join(list.ToArray(), "&ids[]=");
            string endpoint = "api/v2/beatmaps?ids[]=" + p;
            saveSetToArchive(archive, endpoint, accessToken);
        }

        List<string> result = new List<string>();
        foreach (var id in ids)
        {
            foreach (var archiveItem in archive.Items)
            {
                if (id.Equals(archiveItem.id))
                {
                    result.Add(archiveItem.set);
                    break;
                }
            }
        }

        return result;
    }

    private static void saveSetToArchive(FileStore<IdSet> archive, string endpoint, string accessToken)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Add("Authorization", "Bearer " + accessToken);

        var response = client.Send(request);

        Beatmaps result =
            JsonConvert.DeserializeObject<Beatmaps>(response.Content.ReadAsStringAsync().Result);

        foreach (Beatmap resultBeatmap in result.beatmaps)
        {
            archive.Add(new IdSet
            {
                id = resultBeatmap.id.ToString(),
                set = resultBeatmap.beatmapset_id.ToString()
            });
        }

        archive.Store();
    }

    private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
    {
        for (int i = 0; i < locations.Count; i += nSize)
        {
            yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
        }
    }
}