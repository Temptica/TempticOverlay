using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Alerts;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SkipSpeechListener : ISignalRListener
{
    public SkipSpeechListener(HubConnection connection)
    {
        connection.On("SkipSpeech", () =>
        {
            AlertQueue.SkipSpeech();
        });
        
        connection.On("StopAllAlerts", ()=>
        {
            AlertQueue.SkipAll();

        });

    }
}