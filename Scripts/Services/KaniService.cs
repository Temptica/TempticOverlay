using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;
using Temptica.Overlay.Scripts.Models;
using HttpClient = System.Net.Http.HttpClient;

namespace Temptica.Overlay.Scripts.Services;

public partial class KaniService : Node
{
    public static KaniService Instance { get; private set; } = new();

    private NexusAuthService _nexusAuthService;
    private DateTime ExpirationTime => _nexusAuthService?.ExpirationTime ?? DateTime.MinValue;
    private HttpClient _client;

    private string AccessToken
    {
        get
        {
            if (ExpirationTime < DateTime.Now)
            {
                _nexusAuthService.FetchToken().Wait();
            }

            return _nexusAuthService.GetAccessToken();
        }
    }

    public override void _Ready()
    {
        //Authenticate
        _nexusAuthService = GetChild<NexusAuthService>(0);
        _nexusAuthService.Ready += NexusAuthServiceOnReady;
        if (_nexusAuthService.IsReady)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            return;
        }

        _nexusAuthService.Ready += NexusAuthServiceOnReady;
    }

    private void NexusAuthServiceOnReady()
    {
        //Update 
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
    }

    public override void _EnterTree()
    {
        Instance = this;
        _client = new HttpClient();
    }

    public override void _ExitTree()
    {
        Instance = null;
        _client.Dispose();
        _client = null;
    }

    public async Task<List<EggMetaData>> GetAllEggs()
    {
        await CheckToken();
        var request = RequestHelper.CreateRequest(HttpMethod.Get, "eggs");
        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            GD.PushError("Failed to fetch eggs", response.StatusCode, response.ReasonPhrase);
            return [];
        }

        var eggsResponse = await response.Content.ReadFromJsonAsync<List<EggMetaData>>();
        return eggsResponse;
    }

    public async Task<List<string>> GetAllEggNames()
    {
        await CheckToken();

        var request = RequestHelper.CreateRequest(HttpMethod.Get, "eggs?select=name");

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            GD.PushError("Failed to fetch eggs", response.StatusCode, response.ReasonPhrase);
            return [];
        }

        var eggsResponse = await response.Content.ReadFromJsonAsync<List<EggMetaData>>();
        return eggsResponse.Select(e => e.Name).ToList();
    }

    private async Task CheckToken()
    {
        if (ExpirationTime < DateTime.Now)
        {
            await _nexusAuthService.FetchToken();

            if (_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
        }
    }

    public async Task<EggMetaData> GetEggData(string eggName)
    {
        await CheckToken();

        var request = RequestHelper.CreateRequest(HttpMethod.Get, $"eggs?name=eq.{eggName}");
        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            GD.PushError("Failed to fetch egg", response.StatusCode, response.ReasonPhrase);
            return null;
        }

        var eggResponse = await response.Content.ReadFromJsonAsync<List<EggMetaData>>();
        return eggResponse.FirstOrDefault();
    }

    public async Task<bool> CollectEgg(string eggDataName, string twitchId, string userName)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = RequestHelper.GetUri("rpc/collect_egg"),
            Content = new StringContent(
                JsonConvert.SerializeObject(new
                    { egg_name = eggDataName, twitch_id = twitchId, twitch_username = userName }),
                Encoding.UTF8, "application/json")
        };

        await CheckToken();

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode) return true;

        GD.PushError("Failed to collect egg: ", response.ReasonPhrase);
        return false;
    }

    public async Task<List<EggCollection>> GetCollectedEggs(string userId)
    {
        var request = RequestHelper.CreateRequest(HttpMethod.Get, $"eggs_collection?twitch_id=eq.{userId}");

        await CheckToken();
        var response = await _client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var eggs = JsonConvert.DeserializeObject<List<EggCollection>>(json);
        return eggs;
    }


    public async Task<List<EggMetaData>> GetEggsData(params string[] eggNames)
    {
        await CheckToken();
        var request = RequestHelper.CreateRequest(HttpMethod.Get,
            $"egg_data?name=in.({string.Join(",", eggNames.Select(name => $"\"{name}\""))})");

        var response = await _client.SendAsync(request);
        var eggs = await response.Content.ReadFromJsonAsync<List<EggMetaData>>();
        return eggs;
    }

    public async Task<ShoutoutMessage> GetShoutout(string userId)
    {
        await CheckToken();

        var request = RequestHelper.CreateRequest(HttpMethod.Get, $"shoutout?twitch_id=eq.{userId}&select=message");
        var response = await _client.SendAsync(request);
        var data = await response.Content.ReadFromJsonAsync<List<ShoutoutMessage>>();
        return data.FirstOrDefault();
    }

    public async Task SetShoutout(string userId, string message)
    {
        var msg = new ShoutoutMessage()
        {
            TwitchId = userId,
            Message = message,
            Color = "primary",
        };
        
        await CheckToken();
        
        var request = RequestHelper.CreateRequest(HttpMethod.Post, "shoutout?twitch_id=eq." + userId);
        request.Headers.Add("Prefer", "resolution=merge-duplicates");


        request.Content = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json");
        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to set shoutout: {response.StatusCode}: {errorMsg}");
        }
    }
}

file static class RequestHelper
{
    const string ApiUrl = "https://overlay.kani.dev/api/";

    public static Uri GetUri(string path)
    {
        return new Uri(ApiUrl + path);
    }

    public static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
    {
        return new HttpRequestMessage
        {
            Method = method,
            RequestUri = GetUri(endpoint),
        };
    }
}