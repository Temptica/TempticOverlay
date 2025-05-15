using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class StartPartyModeListener : ISignalRListener
{
    
    public static EventHandler StartPartyMode;
    public StartPartyModeListener(HubConnection hubConnection)
    {
        hubConnection.On("StartPartyMode", () =>
        {
            StartPartyMode?.Invoke(this, EventArgs.Empty);
        });
    }
    
}