using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class GenericSignalRListener : ISignalRListener
{
    #region EventHandlers
    
    public static EventHandler<(double current,double goal, string currency)> CharityChanged;

    #endregion

    public GenericSignalRListener(HubConnection connection)
    {
        connection.On<double, double,string>(OverlayHubMethodes.CharityChanged, (current,goal, currency) =>
        {
            GD.Print($"Charity changed to {current}/{goal} {currency}");
            CharityChanged?.Invoke(this,(current,goal,currency));
        });
    }
}