using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Alerts;
using Temptic404Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class AlertListener : ISignalRListener
{
    
    public AlertListener(HubConnection connection)
    {
        connection.On<string, string, AlertType,int>("Alert", (user, message, type, amount) =>
        {
            AlertQueue.AddAlert(new OverlayAlert(user, message, type, amount));
        });
    }
}