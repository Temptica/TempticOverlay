using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class FishMessageListener : ISignalRListener
{
	public static EventHandler<string> FishMessage;
	public FishMessageListener(HubConnection hubConnection)
	{
		hubConnection.On<string,string>("FishMessage", (message, color) =>
		{
			FishMessage?.Invoke(this, message);
		});
	}
	
}
