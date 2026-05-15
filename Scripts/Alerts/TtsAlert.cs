using Godot;

namespace Temptica.Overlay.Scripts.Alerts;

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
		var voiceId = DisplayServer.TtsGetVoicesForLanguage("en")[0];
		DisplayServer.TtsSpeak(TTSMessage, voiceId);
	}

	public override void StopAlert()
	{
		IsFinished = true;
		DisplayServer.TtsStop();
	}
}
