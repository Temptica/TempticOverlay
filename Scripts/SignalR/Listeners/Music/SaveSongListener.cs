using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class SaveSongListener : ISignalRListener
{
    public SaveSongListener(HubConnection hubConnection)
    {
        hubConnection.On("SaveSong", async () => { await SpotifyService.AddToPlayList(); });
    }
}