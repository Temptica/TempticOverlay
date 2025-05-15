using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SetColorListener : ISignalRListener
{
	public static EventHandler<string> SetColor;
	public SetColorListener(HubConnection connection)
	{
		connection.On<string>("SetColor", color =>
		{
			SetColor?.Invoke(this, color.Trim().ToUpper());
		});
	}
	
}
