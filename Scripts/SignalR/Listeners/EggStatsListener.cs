using System;
using System.Collections.Generic;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class EggStatsListener : ISignalRListener
{
    public static EventHandler<(string Name,Dictionary<EggType, int> Eggs)> OnEggStatsReceived;

    public EggStatsListener(HubConnection hubConnection)
    {
        hubConnection.On<string,Dictionary<EggType, int>>("SendCollectedEggs", (user, result) =>
        {
            OnEggStatsReceived?.Invoke(this, (user,result));
        });
        
    }
}