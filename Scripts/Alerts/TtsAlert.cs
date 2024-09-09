using Godot;
using Temptic404Overlay.Scripts.Services;

namespace Temptic404Overlay.Scripts.Alerts;

public class TtsAlert : Alert
{
    public override bool StopMusic => false;

    public TtsAlert(string message) : base("")
    {
        TimeTillTTS = 0;
        TTSMessage = message;
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