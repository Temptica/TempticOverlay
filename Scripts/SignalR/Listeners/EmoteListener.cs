using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class EmoteListener : ISignalRListener
{
    public static EventHandler<int> DisplayBigEmote;
    public static EventHandler<int> DisplayEmote;
    public EmoteListener(HubConnection connection)
    {
        connection.On<int>(OverlayHubMethodes.DisplayBigEmote, emote => 
        {
            DisplayBigEmote?.Invoke(this, emote);
        });
        
        connection.On<int>(OverlayHubMethodes.DisplayEmote, emote => 
        {
            DisplayEmote?.Invoke(this, emote);
        });
    }
}