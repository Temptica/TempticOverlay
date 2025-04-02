using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class GenericSignalRListener : ISignalRListener
{
    #region EventHandlers

    public static EventHandler ThisIsFine; 

    #endregion

    public GenericSignalRListener(HubConnection connection)
    {

        connection.On("ThisIsFine", () =>
        {
            ThisIsFine?.Invoke(this, EventArgs.Empty);
            
        });

    }
}