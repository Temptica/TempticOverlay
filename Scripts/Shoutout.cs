using System;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Services;
using TwitcherSharp;
using TwitcherSharp.Chat;

namespace Temptica.Overlay.Scripts;

public partial class Shoutout : Node
{
    // Called when the node enters the scene tree for the first time.
    private static string Temptic => Overlay.BroadcastUser?.Login ?? "temptic404";
    private static string Kani => "kani_dev";
    public override void _Ready()
    {
        var shoutoutCommand = new TwitchCommand
        {
            Command = "so",
            ListenToChatrooms = [Temptic, Kani],
            ArgsMin = 1,
            ArgsMax = 1,
            PermissionLevel = TwitchCommandBase.PermissionFlag.ModStreamer,
        };

        TwitchService.Instance.AddCommand(shoutoutCommand).CommandReceived += OnShoutoutReceived;
        
        const string shoutoutDescription = "Shoutouts a user with a custom message.";
        
        var setShoutoutCommand = new TwitchCommand
        {
            Command = "setso",
            ListenToChatrooms = [Temptic, Kani],
            Description = shoutoutDescription
        };
        
        TwitchService.Instance.AddCommand(setShoutoutCommand).CommandReceived += OnSetShoutoutReceived;
        
        var setOverrideShoutoutCommand = new TwitchCommand
        {
            Command = "setso",
            ListenToChatrooms = [Temptic, Kani],
            AllowedUsers = [Temptic, Kani],
            Description = shoutoutDescription
        };
        
        TwitchService.Instance.AddCommand(setOverrideShoutoutCommand).CommandReceived += OnSetOverrideShoutoutReceived;
    }

    private static async void OnShoutoutReceived(string s, TwitchCommandInfo twitchCommandInfo, string[] args)
    {
        try
        {
            var username = args[0];
            var user = await TwitchService.Instance.GetUser(username);

            if (user == null)
            {
                return;
            }

            try
            {
                await TwitchBot.Shoutout(Overlay.BroadcastUser, user);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }

            var shoutout = await KaniService.Instance.GetShoutout(user.Id);
            if (shoutout is null) return;

            await TwitchBot.Announcement(shoutout.Message, shoutout.Color);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }

    private static async void OnSetShoutoutReceived(string fromUsername, TwitchCommandInfo info, string[] args)
    {
        if(fromUsername == Temptic) return;
        
        try
        {
            var message = info.TextMessage.Remove(0,7);
            var user = await TwitchService.Instance.GetUser(fromUsername);
        
            if (user == null) return;
        
            await KaniService.Instance.SetShoutout(user.Id, message);
            await TwitchBot.SendMessage("Shoutout set successfully.", info.ChatMessage.MessageId);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            await TwitchBot.SendMessage("Failed to set shoutout.", info.ChatMessage.MessageId);
        }
    }
    
    private static async void OnSetOverrideShoutoutReceived(string fromUsername, TwitchCommandInfo info, string[] args)
    {
        try
        {
            var username = args[0].Replace("@", "");
            if(string.IsNullOrEmpty(username)) throw new Exception("User empty.");
            
            var message = info.TextMessage.Remove(0,7).Replace(username, "").Trim();
            var user = await TwitchService.Instance.GetUser(username);
        
            if (user == null) throw new Exception("User not found.");
        
            await KaniService.Instance.SetShoutout(user.Id, message);
            await TwitchBot.SendMessage("Shoutout set successfully.", info.ChatMessage.MessageId);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            await TwitchBot.SendMessage("Failed to set shoutout.", info.ChatMessage.MessageId);
        }
    }
}
