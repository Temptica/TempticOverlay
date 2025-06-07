using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class TtsListener : ISignalRListener
{
    public TtsListener(HubConnection connection)
    {
        connection.On<string>(OverlayHubMethodes.PlayTts, tts =>
        {
            GD.Print("Recieved TTS: " + tts);
            AlertQueue.AddAlert(new TtsAlert(tts));
        });
    }
    
}