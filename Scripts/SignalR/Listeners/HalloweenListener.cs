using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class HalloweenListener : ISignalRListener
{
    public HalloweenListener(HubConnection connection)
    {
        connection.On("HalloweenSound", HalloweenPlayer.PlayRandomSound);
    }
}