using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.Music;

public class PlaySongListener : ISignalRListener
{
    public PlaySongListener(HubConnection hubConnection)
    {
        hubConnection.On("PlaySong", async (string song) =>
        {
            // Play song
            try
            {
                await hubConnection.InvokeAsync("SendChatMessage", $"Added {SpotifyService.AddSongToQueue(song)} to the queue.");    
            }
            catch (Exception e)
            {
                await hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
    
}