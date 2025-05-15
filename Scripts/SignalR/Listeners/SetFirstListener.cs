using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class SetFirstListener : ISignalRListener
{
	public static EventHandler<string> SetFirst;
	public SetFirstListener(HubConnection connection)
	{
		connection.On<string>("SetFirst", (first) =>
		{
			SetFirst?.Invoke(this, first);
		});
	}
	
}
