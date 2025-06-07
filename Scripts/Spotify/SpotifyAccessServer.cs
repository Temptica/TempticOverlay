using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace Temptica.Overlay.Scripts.Spotify;

internal class SpotifyAccessServer
{

    private static EmbedIOAuthServer _server;
    public bool IsReady;
    private readonly AccessTokenService _accessTokenService;
    private readonly string _callbackUrl = "http://localhost:5005/callback/";
    
    public SpotifyAccessServer(AccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }

    public async Task<AuthorizationCodeTokenResponse> Authenticate()
    {
        var loginRequest = new LoginRequest(new Uri(_callbackUrl), _accessTokenService.GetClientId(), LoginRequest.ResponseType.Code)
        {
            Scope = new List<string> { Scopes.AppRemoteControl, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic, Scopes.PlaylistReadPrivate, Scopes.Streaming
            , Scopes.UserModifyPlaybackState, Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackPosition, Scopes.UserReadPlaybackState, Scopes.UserReadRecentlyPlayed}
        };
        var uri = loginRequest.ToUri();
        BrowserUtil.Open(uri);
        
        //create webserver to listen for the response
        var server = new WebServer(_callbackUrl);
        var auth = await server.Listen();
        server.Stop();
        var response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(_accessTokenService.GetClientId(),
            _accessTokenService.GetClientSecret(), auth.Code, new Uri(_callbackUrl)));
        _accessTokenService.SetSpotifyAccessToken(response.AccessToken);
        _accessTokenService.SetSpotifyRefreshToken(response.RefreshToken);
        return response;
    }
    
    public async Task RefreshToken()
    {
        var tokenResponse = await new OAuthClient().RequestToken(
          new AuthorizationCodeRefreshRequest(_accessTokenService.GetClientId(), _accessTokenService.GetClientSecret(), _accessTokenService.GetSpotifyRefreshToken())
        );
        _accessTokenService.SetSpotifyAccessToken(tokenResponse.AccessToken);
    }
}