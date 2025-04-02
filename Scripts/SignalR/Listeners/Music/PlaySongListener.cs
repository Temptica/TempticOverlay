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
                var songName = await SpotifyService.AddSongToQueue(song);
                
                await hubConnection.InvokeAsync("SendChatMessage", $"Added {songName} to the queue.");
            }
            catch (SongNotFoundException)
            {
                await hubConnection.InvokeAsync("SendChatMessage", "Song not found.");
            }
            catch (Exception e)
            {
                await hubConnection.InvokeAsync("SendChatMessage", e.Message);
            }
        });
    }
    
}