using Godot;
using Temptica.Overlay.Scripts.Services;
using TwitcherSharp;
using TwitcherSharp.Api.Generated;
using TwitcherSharp.Chat;

namespace Temptica.Overlay.Scripts.Commands;

public partial class CommandsManager : Node
{
    private static readonly KaniService KaniService = KaniService.Instance;
    private static readonly TwitchService TwitchService = TwitchService.Instance;
    private static readonly TwitchBot TwitchBot = TwitchBot.Instance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetupSo();
    }

    public static void SetupSo()
    {
        var soCommand = new TwitchCommand
        {
            Aliases = ["shoutout"],
            PermissionLevel = TwitchCommandBase.PermissionFlag.ModStreamer,
            Command = "so",
            ArgsMin = 1,
            ArgsMax = 1,
        };

        TwitchService.AddCommand(soCommand);
        soCommand.CommandReceived += async (_, _, args) =>
        {
            var username = args[0];
            var user = await TwitchService.GetUser(username);
            TwitchService.Shoutout(user);
            
            var msg = await KaniService.GetShoutout(user.Id) ?? $"Check out this amazing streamer at twitch.tv/{username}!";
            await TwitchBot.SendMessage(msg);
        };
    }
}