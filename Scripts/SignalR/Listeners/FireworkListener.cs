using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class FireworkListener : ISignalRListener
{
    public static EventHandler DisplayFirework;

    public FireworkListener(HubConnection connection)
    {
        connection.On("PlayFireworks", () =>
        {
            GD.Print("Fireworks");
            DisplayFirework?.Invoke(this, EventArgs.Empty);
        });

    }
    
}