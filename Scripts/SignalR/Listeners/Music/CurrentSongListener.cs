using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class CurrentSongListener : ISignalRListener
{
    public CurrentSongListener(HubConnection hubConnection)
    {
        hubConnection.On("CurrentSong", () =>
        {
            try
            {
                hubConnection.InvokeAsync("SendChatMessage", $"Currently playing: {SpotifyService.GetCurrentlyPlayingAsync()}");
            }
            catch (SongNotFoundException e)
            {
                hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
    
}