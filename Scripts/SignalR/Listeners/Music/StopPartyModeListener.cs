using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.Music;

public class StopPartyModeListener : ISignalRListener
{
    public static EventHandler StopPartyMode;
    public StopPartyModeListener(HubConnection hubConnection)
    {
        hubConnection.On("StopPartyMode", () =>
        {
            StopPartyMode?.Invoke(this, EventArgs.Empty);
        });
    }
}