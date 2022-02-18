using Newtonsoft.Json;

namespace OsuBeatmapDownloader;

public class FileStore<T>
{
    private List<T> items;

    public List<T> Items
    {
        get => new(items);
        set => items = value;
    }

    private string file;

    public FileStore(string file, bool createFileIfNotExists = true)
    {
        this.file = file;

        FileUtils.PreparePath(file, createFileIfNotExists, createFileIfNotExists);

        using (StreamReader r = new StreamReader(file))
        {
            string json = r.ReadToEnd();
            if (String.IsNullOrEmpty(json))
            {
                items = new List<T>();
            }
            else
            {
                items = JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
    }

    public void Add(T val)
    {
        items.Add(val);
    }

    public void Store()
    {
        using (StreamWriter file = File.CreateText(this.file))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, items);
        }
    }
}