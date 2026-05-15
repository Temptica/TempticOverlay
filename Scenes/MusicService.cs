using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Spotify;
using TwitcherSharp.Chat;
using TwitcherSharp.Extensions;

namespace Temptica.Overlay.Scenes;

public partial class MusicService : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetTwitcherNode<TwitchCommand>("SongRequestCommand")
            .CommandReceived += async (username, info, _) => { await AddSongToQueue(username, info); };
    }

    private static async Task AddSongToQueue(string username, TwitchCommandInfo info)
    {
        if (ExtractTrackId(info.TextMessage, out var trackId))
        {
            var trackResult = await SpotifyService.AddTrackToQueue(trackId, username);

            await TwitchBot.SendMessage(trackResult, info.ChatMessage.MessageId);
            return;
        }

        try
        {
            var result = await SpotifyService.AddSongToQueue(info.TextMessage, username);
            await TwitchBot.SendMessage(result, info.ChatMessage.MessageId);
        }
        catch (Exception e)
        {
            await TwitchChat.Instance.SendMessage(e.Message, info.ChatMessage.MessageId);
        }
    }

    private static bool ExtractTrackId(string input, out string trackId)
    {
        // Handle both URL and URI formats
        if (input.Contains("spotify.com") && input.Contains("track/"))
        {
            // Extract from URL: https://open.spotify.com/track/4iV5W9uYEdYUVa79Axb7Rh?si=...
            var urlParts = input.Split('/');
            for (var i = 0; i < urlParts.Length - 1; i++)
            {
                if (urlParts[i] != "track") continue;

                trackId = urlParts[i + 1].Split('?')[0];
                return true;
            }
        }

        if (input.Contains("spotify:track:"))
        {
            trackId = input.Split(':').LastOrDefault();
            return true;
        }

        trackId = null;
        return false;
    }
}