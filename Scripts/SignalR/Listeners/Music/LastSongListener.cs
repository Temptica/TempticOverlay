using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class LastSongListener : ISignalRListener
{
    public LastSongListener(HubConnection hubConnection)
    {
        hubConnection.On("LastSong", async () =>
        {
            try
            {
                await hubConnection.InvokeAsync("SendChatMessage", $"Last played: {await SpotifyService.GetLastSong()}");
            }
            catch (SongNotFoundException e)
            {
                await hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
    
    
}