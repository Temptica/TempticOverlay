using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class SaveSongListener : ISignalRListener
{
    public SaveSongListener(HubConnection hubConnection)
    {
        hubConnection.On("SaveSong", async () => { await SpotifyService.AddToPlayList(); });
    }
}