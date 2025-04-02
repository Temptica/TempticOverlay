using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpawnBubbleListener : ISignalRListener
{
    public static EventHandler OnSpawnBubble;

    public SpawnBubbleListener(HubConnection hubConnection)
    {
        hubConnection.On("SpawnBubbles", () =>
        {
            OnSpawnBubble?.Invoke(null, EventArgs.Empty);
        });
    }
    
}