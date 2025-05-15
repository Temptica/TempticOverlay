using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SpawnersListener : ISignalRListener
{
    public static EventHandler OnSpawnBubble;
    public static EventHandler OnSpawnBean;
    public SpawnersListener( HubConnection hubConnection )
    {
        hubConnection.On("SpawnBubbles", () =>
        {
            OnSpawnBubble?.Invoke(null, EventArgs.Empty);
        });

        hubConnection.On("Bean", () =>
        {
            GD.Print("OnBean");
            OnSpawnBean?.Invoke(null, EventArgs.Empty);
        });

    }
}