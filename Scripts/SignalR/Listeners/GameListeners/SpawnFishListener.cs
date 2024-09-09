using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpawnFishListener : ISignalRListener
{
    public static EventHandler<int> SpawnFish;
    public SpawnFishListener(HubConnection hubConnection)
    {
        hubConnection.On<int>("SpawnFish", (fish) =>
        {
            SpawnFish?.Invoke(this, fish);
        });
    }
    
}