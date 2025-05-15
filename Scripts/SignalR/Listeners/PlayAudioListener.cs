using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class PlayAudioListener : ISignalRListener
{
	public static EventHandler<string> PlayAudio;
	public PlayAudioListener(HubConnection connection)
	{
		connection.On<string>("PlayAudio", (audio) =>
		{
			PlayAudio?.Invoke(this, audio);
		});
	}
	
}
