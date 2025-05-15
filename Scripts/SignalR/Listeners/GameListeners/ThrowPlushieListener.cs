using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class ThrowPlushieListener : ISignalRListener
{
    public static EventHandler OnThrowPlushie;

    public ThrowPlushieListener(HubConnection hubConnection)
    {
        hubConnection.On("ThrowPlushie", () =>
        {
            OnThrowPlushie?.Invoke(null, EventArgs.Empty);
        });
    }
}