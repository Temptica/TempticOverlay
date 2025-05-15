using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SnowBallListener : ISignalRListener
{
    public static EventHandler SpawnSnowBall;

    public SnowBallListener(HubConnection hubConnection)
    {
        hubConnection.On("ThrowSnowball", () =>
        {
            SpawnSnowBall?.Invoke(this, EventArgs.Empty);
        });
    }
}