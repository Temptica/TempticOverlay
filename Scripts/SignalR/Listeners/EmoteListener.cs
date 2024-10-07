using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class EmoteListener : ISignalRListener
{
    public static EventHandler<int> DisplayBigEmote;
    public static EventHandler<int> DisplayEmote;
    public EmoteListener(HubConnection connection)
    {
        connection.On<int>("DisplayBigEmote", (emote) => 
        {
            GD.Print("Recieved DisplayBigEmote: " + emote);
            DisplayBigEmote?.Invoke(this, emote);
        });
        
        connection.On<int>("DisplayEmote", (emote) => 
        {
            GD.Print("Recieved DisplayEmote: " + emote);
            DisplayEmote?.Invoke(this, emote);
        });
    }
}