using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class CurrentSongListener : ISignalRListener
{
    public CurrentSongListener(HubConnection hubConnection)
    {
        hubConnection.On("CurrentSong",async  () =>
        {
            try
            {
                await hubConnection.InvokeAsync("SendChatMessage", $"Currently playing: {await SpotifyService.GetCurrentlyPlayingAsync()}");
            }
            catch (SongNotFoundException e)
            {
                await hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
}