using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class OverlayClickListener : ISignalRListener
{
    public static EventHandler<OverlayClickModel> OverlayClick;
    public OverlayClickListener(HubConnection hubConnection)
    {
        hubConnection.On<string,string,string,string>(OverlayHubMethodes.OnOverlayClick, (x,y,username,color) =>
        {
            var overlayClickModel = new OverlayClickModel(x,y,username,color);
            OverlayClick?.Invoke(this, overlayClickModel);
        });
    }
}