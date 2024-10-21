using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Models;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpinListener : ISignalRListener
{
    public SpinListener(HubConnection hubConnection)
    {
        hubConnection.On<string,int>("SpinSlotMachine", SlotMachine.Spin);
        
    }
}