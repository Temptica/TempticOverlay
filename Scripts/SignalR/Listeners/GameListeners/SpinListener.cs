using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class SpinListener : ISignalRListener
{
    public SpinListener(HubConnection hubConnection)
    {
        hubConnection.On<string,int>(OverlayHubMethodes.SpinSlotMachine, SlotMachine.Spin);
        
    }
}