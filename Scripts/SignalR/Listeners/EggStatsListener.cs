using System;
using System.Collections.Generic;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Alerts;
using Temptic404Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class EggStatsListener : ISignalRListener
{
    public static EventHandler<(string Name,Dictionary<EggType, int> Eggs)> OnEggStatsReceived;

    public EggStatsListener(HubConnection hubConnection)
    {
        hubConnection.On<string,Dictionary<EggType, int>>("SendCollectedEggs", (user, result) =>
        {
            GD.Print("Egg stats received: " + result);
            OnEggStatsReceived?.Invoke(this, (user,result));
        });
        
    }
}