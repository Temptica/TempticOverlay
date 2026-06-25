using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using SpotifyAPI.Web;
using TwitcherSharp.Chat;

namespace Temptica.Overlay.Scripts.Spotify;

public class SpotifyService(AccessTokenService accessTokenService)
{
    private static SpotifyClient _spotify;
    private static bool _paused;
    private static bool _stopped;
    private static string _lastSong = "";
    private const int PollRate = 2;

    private static KeyValuePair<string, string>? CurrentSongRequest { get; set; } = new(); //<Song name, Username>
    private static Dictionary<string, SongRequest> SongRequestQueue { get; } = new(); //<Song name, Username>
    private static List<string> SongsToSkip { get; } = [];

    public static EventHandler<string> SongChanged { get; set; }

    private static int _songIndex = 0;


    public async Task Initialize()
    {
        if (!accessTokenService.IsLoaded())
        {
            accessTokenService.LoadTokens();
        }

        var accessServer = new SpotifyAccessServer(accessTokenService);
        //authenticate with spotify with the stored access token, if failed, refresh token
        if (!string.IsNullOrEmpty(accessTokenService.GetSpotifyAccessToken()))
        {
            _spotify ??= new SpotifyClient(SpotifyClientConfig
                .CreateDefault(accessTokenService.GetSpotifyAccessToken())
                .WithAuthenticator(new AuthorizationCodeAuthenticator(accessTokenService.GetClientId(),
                    accessTokenService.GetClientSecret(), new AuthorizationCodeTokenResponse
                    {
                        AccessToken = accessTokenService.GetSpotifyAccessToken(),
                        RefreshToken = accessTokenService.GetSpotifyRefreshToken()
                    })));
        }

        try
        {
            if (!string.IsNullOrEmpty(accessTokenService.GetSpotifyAccessToken()))
            {
                GetCurrentlyPlayingAsync().Wait();
            }
            else
            {
                throw new Exception("No access token");
            }
        }
        catch (Exception)
        {
            if (!string.IsNullOrEmpty(accessTokenService.GetSpotifyRefreshToken()) &&
                !string.IsNullOrEmpty(accessTokenService.GetSpotifyAccessToken()))
            {
                accessServer.RefreshToken().Wait();

                _spotify = new SpotifyClient(SpotifyClientConfig
                    .CreateDefault(accessTokenService.GetSpotifyAccessToken())
                    .WithAuthenticator(new AuthorizationCodeAuthenticator(accessTokenService.GetClientId(),
                        accessTokenService.GetClientSecret(), new AuthorizationCodeTokenResponse
                        {
                            AccessToken = accessTokenService.GetSpotifyAccessToken(),
                            RefreshToken = accessTokenService.GetSpotifyRefreshToken()
                        })));
            }

            try
            {
                if (!string.IsNullOrEmpty(accessTokenService.GetSpotifyRefreshToken()) &&
                    !string.IsNullOrEmpty(accessTokenService.GetSpotifyAccessToken()))
                {
                    await GetCurrentlyPlayingAsync();
                }
                else
                {
                    throw new Exception("No access token");
                }
            }
            catch (SongNotFoundException)
            {
                _stopped = true;
            }
            catch (Exception)
            {
                var response = accessServer.Authenticate()
                    .Result;
                _spotify = new SpotifyClient(SpotifyClientConfig
                    .CreateDefault()
                    .WithAuthenticator(
                        new AuthorizationCodeAuthenticator(accessTokenService.GetClientId(),
                            accessTokenService.GetClientSecret(), response)));
            }
        }

        _ = GetCurrentlyPlayingLoopAsync();
        accessTokenService.SaveTokens();
    }

    /// <summary>
    /// Gets the song that is currently playing
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SongNotFoundException"></exception>
    public static async Task<string> GetCurrentlyPlayingAsync()
    {
        var currentlyPlaying =
            await _spotify.Player.GetCurrentlyPlaying(
                new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));

        if (CurrentlyPlayingAsync(currentlyPlaying, out var s)) return s;

        throw new SongNotFoundException("Nothing is currently playing");
    }

    private static bool CurrentlyPlayingAsync(CurrentlyPlaying currentlyPlaying, out string resultString)
    {
        resultString = "";

        if (currentlyPlaying.Item is not FullTrack track) return false;

        var trackName = GetTrackName(track);

        if (CurrentSongRequest != null && CurrentSongRequest.Value.Key == trackName)
        {
            resultString = trackName + $" requested by {CurrentSongRequest.Value.Value}";
            return true;
        }

        if (SongsToSkip.Contains(trackName))
        {
            _ = Skip();
            SongsToSkip.Remove(trackName);
        }

        if (SongRequestQueue.TryGetValue(trackName, out var value))
        {
            if (CurrentSongRequest is { Key: not null } && CurrentSongRequest.Value.Key != value.SongName)
            {
                SongRequestQueue.Remove(CurrentSongRequest.Value.Key);
            }

            CurrentSongRequest = new KeyValuePair<string, string>(trackName, value.SongName);

            resultString = trackName + $" requested by {CurrentSongRequest.Value.Value}";

            return true;
        }

        resultString = trackName;
        return true;
    }

    //loop every 10 seconds to see if the song has changed. if so raise an event to update the song in the overlay
    private async Task GetCurrentlyPlayingLoopAsync()
    {
        //calls GetCurrentlyPlaying() every 10 seconds
        //run a timer that calls GetCurrentlyPlaying() every 10 seconds
        var timer = new PeriodicTimer(new TimeSpan(0, 0, 0, PollRate));

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                var currentlyPlaying =
                    await _spotify.Player.GetCurrentlyPlaying(
                        new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));

                if (CurrentlyPlayingAsync(currentlyPlaying, out var s))
                {
                    if (s == _lastSong && (_paused || !_lastSong.Contains("Paused"))) continue;

                    _lastSong = s;
                    SongChanged?.Invoke(this, s);

                    continue;
                }

                SongChanged?.Invoke(this, "Not playing anything");
            }
            catch (SongNotFoundException)
            {
                if (_paused && !_lastSong.Contains("Paused"))
                {
                    SongChanged?.Invoke(this, "Not playing anything");
                }
            }
            catch (Exception)
            {
                if (_paused && !_lastSong.Contains("Paused"))
                {
                    SongChanged?.Invoke(this, "Not playing anything");
                }
            }
        }
    }

    /// <summary>
    /// Add song name to queue
    /// </summary>
    /// <param name="songName">song name</param>
    /// <param name="username">name of the user who requested it</param>
    /// <returns></returns>
    /// <exception cref="SongNotFoundException"></exception>
    public static async Task<string> AddSongToQueue(string songName, string username)
    {
        try
        {
            var result = await _spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, songName));
            if (result.Tracks.Items?[0] is null) throw new SongNotFoundException("No songs found");

            try
            {
                var track = result.Tracks.Items[0];

                var req = new PlayerAddToQueueRequest(track.Uri);
                var trackName = GetTrackName(track);

                if (!SongRequestQueue.ContainsKey(trackName))
                {
                    await _spotify.Player.AddToQueue(req);
                    var request = new SongRequest(++_songIndex, trackName, username);
                    SongRequestQueue.Add(trackName, request);
                    return $"Added {trackName} to queue (id: {request.Id})";
                }

                var existingQueueNumber = SongRequestQueue
                    .Where(kvp => kvp.Value.SongName == trackName)
                    .Select(kvp => kvp.Value.Id)
                    .FirstOrDefault();

                return $"{songName} could not be added to queue as it already exists: ({existingQueueNumber}).";
            }
            catch (Exception)
            {
                return $"{songName} Could not be added."; //sometimes add to que does add while throwing an error
            }
        }
        catch (Exception e)
        {
            GD.Print(e.Message);
        }

        return $"{songName} Could not be added.";
    }

    public static async Task Pause()
    {
        try
        {
            _paused = true;
            var playing = await GetCurrentlyPlayingAsync();
            SongChanged?.Invoke(null, playing + " | Paused");
        }
        catch (SongNotFoundException)
        {
            SongChanged?.Invoke(null, "Paused");
        }

        try
        {
            await _spotify.Player.PausePlayback();
        }
        catch (Exception e)
        {
            // ignored
            GD.Print($"Problem pausing {e.Message}");
        }
    }

    public static async Task Resume()
    {
        try
        {
            if (_stopped) return;
            await _spotify.Player.ResumePlayback();
            SongChanged?.Invoke(null, _lastSong.Replace("Paused", ""));
            _paused = false;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static async Task Stop()
    {
        _stopped = true;
        await Pause();
    }

    public static async Task Start()
    {
        _stopped = false;
        await Resume();
    }

    public static async Task AddToPlayList(string playlist = "Stream")
    {
        var currentlyPlaying =
            await _spotify
                .Player
                .GetCurrentlyPlaying(
                    new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));
        if (currentlyPlaying.Item is FullTrack track)
        {
            var playlists = _spotify.Playlists.CurrentUsers(new PlaylistCurrentUsersRequest { Limit = 30 }).Result;
            var id = playlists.Items?.Find(pl => pl.Name == playlist)?.Id;
            if (id is not null)
            {
                var req2 = new PlaylistAddItemsRequest(new List<string> { track.Uri });
                await _spotify.Playlists.AddItems(id, req2);
                return;
            }

            id = playlists.Items?.Find(pl => pl.Name == "Stream")?.Id;
            if (id is not null)
            {
                var req3 = new PlaylistAddItemsRequest(new List<string> { track.Uri });
                await _spotify.Playlists.AddItems(id, req3);
            }
        }
    }

    public static async Task Skip()
    {
        await _spotify.Player.SkipNext();
    }

    public static async Task Previous()
    {
        await _spotify.Player.SkipPrevious();
    }

    /// <summary>
    /// Retrieves last played song
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SongNotFoundException"></exception>
    public static async Task<string> GetLastSong()
    {
        var lastSong = (await _spotify.Player.GetRecentlyPlayed()).Items?.First();
        if (lastSong is not null)
        {
            return GetTrackName(lastSong.Track);
        }

        throw new SongNotFoundException("Nothing is currently playing or has been played");
    }

    private static string GetTrackName(FullTrack track)
    {
        if (track.Artists.Count < 2)
            return $"{track.Name} by {track.Artists.First().Name}";
        var artists = string.Join(", ", track.Artists.Select(artist => artist.Name).ToList());
        return $"{track.Name} by {artists}";
    }

    public static async Task PlaySong(string songId)
    {
        await _spotify.Player.AddToQueue(new PlayerAddToQueueRequest("spotify:track:" + songId));
    }

    public static void SkipLastRequest(string username, string messageId = null)
    {
        try
        {
            var lastRequest = SongRequestQueue.Last(kvp => kvp.Value.Username == username);

            SongRequestQueue.Remove(lastRequest.Key);
            SongsToSkip.Add(lastRequest.Key);
            _ = TwitchChat.Instance.SendMessage(
                $"@{username}: {lastRequest.Key} will be skipped! (Might take a few seconds)", messageId);
        }
        catch (Exception)
        {
            _ = TwitchChat.Instance.SendMessage("You don't have any songs in the queue.", messageId);
        }
    }

    public static async Task<string> AddTrackToQueue(string trackId, string username)
    {
        await PlaySong(trackId);
        var track = GetTrackName(await _spotify.Tracks.Get(trackId));
        var request = new SongRequest(++_songIndex, track, username);
        SongRequestQueue.Add(track, request);

        return $"Added {track} to queue (id: {request.Id})";
    }

    public static void SkipById(int result, string fromUsername, string chatMessageMessageId)
    {
        var song = SongRequestQueue.FirstOrDefault(kvp => kvp.Value.Id == result).Value;
        if (song is null || song.Username != fromUsername)
        {
            _ = TwitchChat.Instance.SendMessage("You don't have permission to skip this song.", chatMessageMessageId);
            return;
        }

        SongRequestQueue.Remove(song.SongName);
        SongsToSkip.Add(song.SongName);
        _ = TwitchChat.Instance.SendMessage(
            $"{song.SongName} will be skipped! (Might take a few seconds)", chatMessageMessageId);
    }
}

public class SongNotFoundException(string message = "") : Exception(message);

public record SongRequest(int Id, string SongName, string Username);