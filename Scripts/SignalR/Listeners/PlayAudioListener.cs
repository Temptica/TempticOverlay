using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class PlayAudioListener : ISignalRListener
{
	public static EventHandler<string> PlayAudio;
	public PlayAudioListener(HubConnection connection)
	{
		connection.On<string>(OverlayHubMethodes.PlayAudio, audio =>
		{
			PlayAudio?.Invoke(this, audio);
		});
	}
	
}
