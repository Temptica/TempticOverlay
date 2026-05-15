using System.IO;
using Godot;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

namespace Temptica.Overlay.Scripts.Spotify;

public class AccessTokenService
{
    private string _spotifyAccessToken = "";
    private string _spotifyRefreshToken = "";
    private string _clientId = "";
    private string _clientSecret = "";
    private bool _loaded;

    public string GetSpotifyAccessToken()
    {
        return _spotifyAccessToken;
    }

    public void SetSpotifyAccessToken(string token)
    {
        _spotifyAccessToken = token;
    }

    public string GetSpotifyRefreshToken()
    {
        return _spotifyRefreshToken;
    }

    public void SetSpotifyRefreshToken(string token)
    {
        _spotifyRefreshToken = token;
    }

    public string GetClientId()
    {
        return _clientId;
    }

    public string GetClientSecret()
    {
        return _clientSecret;
    }

    public void SaveTokens()
    {
        // Save tokens to file
        //convert to json and save to project folder as tokens.json
        var accessTokens = new AccessTokens
        {
            SpotifyAccessToken = _spotifyAccessToken,
            SpotifyRefreshToken = _spotifyRefreshToken,
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };
        var json = JsonConvert.SerializeObject(accessTokens);
        ValidateFileExist();
        var file = FileAccess.Open("user://tokens.json", FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }

    private static void ValidateFileExist()
    {
        if (!FileAccess.FileExists("user://tokens.json"))
        {
            FileAccess.Open("user://tokens.json", FileAccess.ModeFlags.Write);
        }
    }

    public void LoadTokens()
    {
        // Load tokens from file
        //read from project folder tokens.json and convert to object
        ValidateFileExist();
        var json = FileAccess.Open("user://tokens.json", FileAccess.ModeFlags.Read).GetAsText();
        var tokens = JsonConvert.DeserializeObject<AccessTokens>(json);
        if (tokens is null)
        {
            json = File.ReadAllText("tokens.json");
            tokens = JsonConvert.DeserializeObject<AccessTokens>(json);
            if (tokens is null)
            {
                GD.PrintErr("Failed to load tokens");
                return;
            }
        }

        _loaded = true;
        _spotifyAccessToken = tokens.SpotifyAccessToken;
        _spotifyRefreshToken = tokens.SpotifyRefreshToken;
        _clientId = tokens.ClientId;
        _clientSecret = tokens.ClientSecret;
    }

    public bool IsLoaded()
    {
        return _loaded;
    }

    public class AccessTokens
    {
        public string SpotifyAccessToken { get; set; }
        public string SpotifyRefreshToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}