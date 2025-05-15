using System;
using Godot;
using Temptica.Overlay.Scripts.Extensions;
using Temptica.Overlay.Scripts.Services;

namespace Temptica.Overlay.Scripts.Alerts;

public class TtsAlert : Alert
{
    public override bool StopMusic => false;

    public TtsAlert(string message) : base("")
    {
        TimeTillTTS = 0;
        TTSMessage = message.CleanEmoteName();
        Duration = 2;
    }

    protected override void StartAlert()
    {
        //does not show alert. Start TTS asap
        TextToSpeechService.Instance.Speak(TTSMessage);
    }

    public override void StopAlert()
    {
        IsFinished = true;
        TextToSpeechService.Stop();
    }
}