using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SetColorListener : ISignalRListener
{
	public static EventHandler<string> SetColor;
	public SetColorListener(HubConnection connection)
	{
		connection.On<string>(OverlayHubMethodes.SetColor, color =>
		{
			SetColor?.Invoke(this, color.Trim().ToUpper());
		});
	}
	
}
