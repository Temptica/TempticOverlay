using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class PlaySongListener : ISignalRListener
{
    public PlaySongListener(HubConnection hubConnection)
    {
        hubConnection.On("PlaySong", async (string song, string username) =>
        {
            // Play song
            try
            {
                var result = await SpotifyService.AddSongToQueue(song,username);
                
                await hubConnection.InvokeAsync("SendChatMessage", result);
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