using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class OtterSignalRListener : ISignalRListener
{
    public static EventHandler ThisIsFine;
    public static EventHandler Squish;
    public static EventHandler Pixelate;

    public OtterSignalRListener(HubConnection connection)
    {
        connection.On("ThisIsFine", () => { ThisIsFine?.Invoke(this, EventArgs.Empty); });
        connection.On("Squish", () => { Squish?.Invoke(this, null!); });
        connection.On<string>("Pixelate", _ => { Pixelate?.Invoke(this, null!); });
    }
}