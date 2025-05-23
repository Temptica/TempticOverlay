using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class StartFishGameListener : ISignalRListener
{
    public static EventHandler StartFishGame;
    public StartFishGameListener(HubConnection hubConnection)
    {
        hubConnection.On("StartFishGame", () =>
        {
            StartFishGame?.Invoke(this, EventArgs.Empty);
        });
    }
    
}