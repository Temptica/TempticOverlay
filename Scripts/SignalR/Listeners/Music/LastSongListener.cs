using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class LastSongListener : ISignalRListener
{
    public LastSongListener(HubConnection hubConnection)
    {
        hubConnection.On("LastSong", () =>
        {
            try
            {
                hubConnection.InvokeAsync("SendChatMessage", $"Last played: {SpotifyService.GetLastSong()}");
            }
            catch (SongNotFoundException e)
            {
                hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
    
    
}