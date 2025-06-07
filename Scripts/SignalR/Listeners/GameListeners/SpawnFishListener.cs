using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpawnFishListener : ISignalRListener
{
    public static EventHandler<int> SpawnFish;
    public SpawnFishListener(HubConnection hubConnection)
    {
        hubConnection.On<int>(OverlayHubMethodes.SpawnFish, fish =>
        {
            SpawnFish?.Invoke(this, fish);
        });
    }
    
}