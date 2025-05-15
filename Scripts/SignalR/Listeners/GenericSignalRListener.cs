using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class GenericSignalRListener : ISignalRListener
{
    #region EventHandlers

    public static EventHandler ThisIsFine; 
    public static EventHandler Squish;

    #endregion

    public GenericSignalRListener(HubConnection connection)
    {

        connection.On("ThisIsFine", () =>
        {
            ThisIsFine?.Invoke(this, EventArgs.Empty);
            
        });
        
        connection.On("Squish", () =>
        {
            Squish?.Invoke(this,null!);
        });

    }
}