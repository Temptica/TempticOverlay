using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class NorthPoleLightListener : ISignalRListener
{
    public static EventHandler Lights;
    
    public NorthPoleLightListener(HubConnection connection)
    {
        connection.On("NorthPoleLight", () =>
        {
            Lights?.Invoke(this, EventArgs.Empty);
        });
    }
    
}