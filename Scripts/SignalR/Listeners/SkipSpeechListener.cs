using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Alerts;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class SkipSpeechListener : ISignalRListener
{
    public SkipSpeechListener(HubConnection connection)
    {
        connection.On("SkipSpeech", () =>
        {
            GD.Print("Recieved SkipSpeech");
            AlertQueue.SkipSpeech();
        });
        
        connection.On("StopAllAlerts", ()=>
        {
            GD.Print("Recieved StopAllAlerts");
            AlertQueue.SkipAll();

        });

    }
}