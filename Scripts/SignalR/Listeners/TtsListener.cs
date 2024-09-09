using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Alerts;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class TtsListener : ISignalRListener
{
    public TtsListener(HubConnection connection)
    {
        connection.On<string>("PlayTts", (tts) =>
        {
            GD.Print("Recieved TTS: " + tts);
            AlertQueue.AddAlert(new TtsAlert(tts));
        });
    }
    
}