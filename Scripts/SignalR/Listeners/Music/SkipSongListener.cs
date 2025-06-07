using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Spotify;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class SkipSongListener : ISignalRListener
{
    public SkipSongListener(HubConnection hubConnection)
    {
        hubConnection.On("SkipSong", async () =>
        {
            // Skip song
            await SpotifyService.Skip();
        });

        hubConnection.On<string>(OverlayHubMethodes.SkipLastSong, SpotifyService.SkipLastRequest);
    }
}