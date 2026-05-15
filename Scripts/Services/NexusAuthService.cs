using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;
using HttpClient = System.Net.Http.HttpClient;

namespace Temptica.Overlay.Scripts.Services;

public partial class NexusAuthService : Node
{
    public const string NexusSettingsPath = "res://addons/scene_nexus/nexus_settings.tres";

    public Resource NexusSettings { get; private set; }
    public DateTime ExpirationTime { get; private set; }

    private string _accessToken;

    public bool IsReady => !string.IsNullOrEmpty(_accessToken);

    [Signal]
    public delegate void ReadyEventHandler();

    public override void _Ready()
    {
        NexusSettings = GD.Load<Resource>(NexusSettingsPath);
        _ = FetchToken();
    }

    public string GetAccessToken()
    {
        return _accessToken;
    }

    public async Task FetchToken()
    {
        var client = new HttpClient();

        var body =
            $"grant_type=client_credentials&client_id={NexusSettings.Get("client_id").AsString().URIEncode()}&client_secret={NexusSettings.Get("client_secret").AsString().URIEncode()}";

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(NexusSettings.Get("token_url").AsString()),
            Content = new StringContent(body),
        };

        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var result = await client.SendAsync(requestMessage);
        if (!result.IsSuccessStatusCode) GD.PrintErr("Failed to fetch token for Nexus");

        var responseString = await result.Content.ReadAsStringAsync();
        var response =  JsonConvert.DeserializeObject<NexusAuthResponse>(responseString);
        _accessToken = response.AccessToken;
        ExpirationTime = DateTime.Now.AddSeconds(response.ExpiresIn-5);

        EmitSignalReady();
    }
}

public class NexusAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    
}