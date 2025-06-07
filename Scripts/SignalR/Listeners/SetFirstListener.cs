using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SetFirstListener : ISignalRListener
{
	public static EventHandler<string> SetFirst;
	public SetFirstListener(HubConnection connection)
	{
		connection.On<string>(OverlayHubMethodes.SetFirst, first =>
		{
			SetFirst?.Invoke(this, first);
		});
	}
	
}
