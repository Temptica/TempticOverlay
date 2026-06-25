using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Spotify;
using TwitcherSharp;
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

        var skipLastCommand = new TwitchCommand()
        {
            Command = "skipLast",
            PermissionLevel = TwitchCommandBase.PermissionFlag.Vip | TwitchCommandBase.PermissionFlag.ModStreamer,
            Description = "Skips the last song you added to the queue",
        };
        
        TwitchService.Instance.AddCommand(skipLastCommand).CommandReceived += OnSkipLastCommandReceived;

        var skipCommand = new TwitchCommand()
        {
            Command = "skip",
            PermissionLevel = TwitchCommandBase.PermissionFlag.ModStreamer,
            Description = "Skips the current song",
        };
        
        TwitchService.Instance.AddCommand(skipCommand).CommandReceived += OnSkipReceived;
        
        var skipSpecificCommand = new TwitchCommand()
        {
            Command = "skipSong",
            PermissionLevel = TwitchCommandBase.PermissionFlag.ModStreamer,
            Description = "Skips the current song",
            ArgsMin = 1,
            ArgsMax = 1
        };
        
        TwitchService.Instance.AddCommand(skipSpecificCommand).CommandReceived += OnSkipSpecificSongReceived;
        
    }

    private void OnSkipSpecificSongReceived(string fromUsername, TwitchCommandInfo info, string[] args)
    {
        var songId = args[0];
        if (!int.TryParse(songId, out var id))
        {
            _ = TwitchBot.SendMessage("Invalid song id", info.ChatMessage.MessageId);
        }

        SpotifyService.SkipById(id, fromUsername, info.ChatMessage.MessageId);

    }

    private void OnSkipReceived(string fromUsername, TwitchCommandInfo info, string[] args)
    {
        _ = SpotifyService.Skip();
    }

    private void OnSkipLastCommandReceived(string fromUsername, TwitchCommandInfo info, string[] args)
    {
        SpotifyService.SkipLastRequest(fromUsername, info.ChatMessage.MessageId);
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