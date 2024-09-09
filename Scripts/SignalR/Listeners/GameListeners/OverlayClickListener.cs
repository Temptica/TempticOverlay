using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Models;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class OverlayClickListener : ISignalRListener
{
    public static EventHandler<OverlayClickModel> OverlayClick;
    public OverlayClickListener(HubConnection hubConnection)
    {
        hubConnection.On<string,string,string,string>("OverlayClick", (x,y,username,color) =>
        {
            var overlayClickModel = new OverlayClickModel(x,y,username,color);
            OverlayClick?.Invoke(this, overlayClickModel);
        });
    }
}