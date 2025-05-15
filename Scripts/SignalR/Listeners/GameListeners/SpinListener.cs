using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Models;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpinListener : ISignalRListener
{
    public SpinListener(HubConnection hubConnection)
    {
        hubConnection.On<string,int>("SpinSlotMachine", SlotMachine.Spin);
        
    }
}