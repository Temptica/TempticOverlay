using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Temptic404Overlay.Scripts.Spotify;

public class SpotifyService
{
    private readonly AccessTokenService _accessTokenService;
    private static SpotifyClient _spotify;
    private static bool _paused;
    static string _lastSong = "";

    public static EventHandler<string> SongChanged { get; set; }

    public SpotifyService(AccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }
    
    public async Task Initialize()
    {
        if (!_accessTokenService.IsLoaded())
        {
            _accessTokenService.LoadTokens();
        }
        var accessServer = new SpotifyAccessServer(_accessTokenService);
        //authenticate with spotify with the stored access token, if failed, refresh token
        if (!string.IsNullOrEmpty(_accessTokenService.GetSpotifyAccessToken()))
        {
            _spotify ??= new SpotifyClient(SpotifyClientConfig
                .CreateDefault(_accessTokenService.GetSpotifyAccessToken())
                .WithAuthenticator(new AuthorizationCodeAuthenticator(_accessTokenService.GetClientId(),_accessTokenService.GetClientSecret(), new AuthorizationCodeTokenResponse()
                {
                    AccessToken = _accessTokenService.GetSpotifyAccessToken(),
                    RefreshToken = _accessTokenService.GetSpotifyRefreshToken()
                })));
            
        }
        try
        {
            if (!string.IsNullOrEmpty(_accessTokenService.GetSpotifyAccessToken()))
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

            if (!string.IsNullOrEmpty(_accessTokenService.GetSpotifyRefreshToken()) &&
                !string.IsNullOrEmpty(_accessTokenService.GetSpotifyAccessToken()))
            {
                

                accessServer.RefreshToken().Wait();
            
                _spotify = new SpotifyClient(SpotifyClientConfig
                    .CreateDefault(_accessTokenService.GetSpotifyAccessToken())
                    .WithAuthenticator(new AuthorizationCodeAuthenticator(_accessTokenService.GetClientId(),_accessTokenService.GetClientSecret(), new AuthorizationCodeTokenResponse()
                    {
                        AccessToken = _accessTokenService.GetSpotifyAccessToken(),
                        RefreshToken = _accessTokenService.GetSpotifyRefreshToken()
                    })));
            }
            
            try
            {
                if (!string.IsNullOrEmpty(_accessTokenService.GetSpotifyRefreshToken()) &&
                    !string.IsNullOrEmpty(_accessTokenService.GetSpotifyAccessToken()))
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
                var response = accessServer.Authenticate()
                    .Result;
                _spotify = new SpotifyClient(SpotifyClientConfig
                    .CreateDefault()
                    .WithAuthenticator(
                        new AuthorizationCodeAuthenticator(_accessTokenService.GetClientId(), _accessTokenService.GetClientSecret(), response)));
            }
        }
        _ = GetCurrentlyPlayingLoopAsync();
        _accessTokenService.SaveTokens();
    }
    
    
    
    /// <summary>
    /// Gets the song that is currently playing
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SongNotFoundException"></exception>
    public static async Task<string> GetCurrentlyPlayingAsync()
    {
        var currentlyPlaying = 
            await _spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));

        if (currentlyPlaying.Item is FullTrack track)
        {
            return GetTrackName(track);
        }
        
        throw new SongNotFoundException("Nothing is currently playing");
        
    }
    //loop every 10 seconds to see if the song has changed. if so raise an event to update the song in the overlay
    private async Task GetCurrentlyPlayingLoopAsync()
    {
        //calls GetCurrentlyPlaying() every 10 seconds
        //run a timer that calls GetCurrentlyPlaying() every 10 seconds

        var timer = new PeriodicTimer(new TimeSpan(0, 0, 0, 10));
       
        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                var currentlyPlaying = 
                    await _spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));
                var song = "Not playing anything";
                if (currentlyPlaying?.Item is FullTrack track)
                {
                    song = GetTrackName(track);
                }
                if (song != _lastSong)
                {
                    _lastSong = song;
                    SongChanged?.Invoke(this, song);
                }
            }
            catch (SongNotFoundException)
            {
                if (_paused && !_lastSong.Contains("Paused"))
                {
                    SongChanged?.Invoke(this, "Not playing anything");
                }
            }
            catch(Exception)
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
    /// <param name="name"> song name</param>
    /// <returns></returns>
    /// <exception cref="SongNotFoundException"></exception>
    public static async Task<string> AddSongToQueue(string name)
    {
        var result = await _spotify.Search.Item(new(SearchRequest.Types.Track, name));
        if (result.Tracks.Items is null) throw new SongNotFoundException("No songs found");
        
        var track = result.Tracks.Items.First();
        var req = new PlayerAddToQueueRequest(track.Uri);
        await _spotify.Player.AddToQueue(req);
        
        return GetTrackName(track);
    }
    
    public static async Task Pause()
    {
        try
        {
            _paused = true;
            var playing = await GetCurrentlyPlayingAsync();
            SongChanged?.Invoke(null,playing + " | Paused");
        }
        catch (SongNotFoundException)
        {
            SongChanged?.Invoke(null, "Paused");
        }

        try
        {
            await _spotify.Player.PausePlayback();
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    public static async Task Resume()
    {
        try
        {
            await _spotify.Player.ResumePlayback();
            var playing = await GetCurrentlyPlayingAsync();
            SongChanged?.Invoke(null, playing);
            _paused = false;
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    public static async Task AddToPlayList(string playlist = "Stream")
    {
        var currentlyPlaying = 
            await _spotify
                .Player
                .GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));
        if (currentlyPlaying.Item is FullTrack track)
        {
            var playlists = _spotify.Playlists.CurrentUsers( new PlaylistCurrentUsersRequest { Limit = 30}).Result;
            var id = playlists.Items?.Find(pl => pl.Name == playlist)?.Id;
            if(id is not null)
            {
                var req2 = new PlaylistAddItemsRequest(new List<string>() { track.Uri });
                await _spotify.Playlists.AddItems(id, req2);
                return;
            }
            id = playlists.Items?.Find(pl => pl.Name == "Stream")?.Id;
            if (id is not null)
            {
                var req3 = new PlaylistAddItemsRequest(new List<string>() { track.Uri });
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
        var artists = track.Artists.Aggregate("", (current, artist) => current + artist.Name + ", ");
        artists = artists.Remove(artists.LastIndexOf(','));
        return $"{track.Name} by {artists}";
    }

    public static async Task PlaySong(string songId)
    {
        await _spotify.Player.AddToQueue(new PlayerAddToQueueRequest("spotify:track:"+songId));
    }
}
public class SongNotFoundException : Exception
{
    public SongNotFoundException(string message = "") : base(message)
    {
    }
}

