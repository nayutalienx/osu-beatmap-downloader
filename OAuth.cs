using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace OsuBeatmapDownloader;

public class OAuth
{
    static string url = "https://osu.ppy.sh/";
    static string endpoint = "oauth/token";

    class Token
    {
        public string access_token { get; set; }
    }

    public static string GetAccessToken(string username, string password)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new StringContent(getBody(username, password), Encoding.UTF8, "application/json");

        var response = client.Send(request);

        Token result = JsonConvert.DeserializeObject<Token>(response.Content.ReadAsStringAsync().Result);

        return result.access_token;
    }


    private static string getBody(string username, string password)
    {
        return
            "{\"client_id\": 5, \"client_secret\":\"FGc9GAtyHzeQDshWP5Ah7dega8hJACAJpQtw6OXk\",\"grant_type\":\"password\",\"scope\":\"*\", \"username\": \"" +
            username + "\", \"password\": \"" + password + "\"}";
    }
}