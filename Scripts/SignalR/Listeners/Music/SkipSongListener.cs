using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class SkipSongListener : ISignalRListener
{
    public SkipSongListener(HubConnection hubConnection)
    {
        hubConnection.On("SkipSong", async () =>
        {
            // Skip song
            await SpotifyService.Skip();
        });
    }
}