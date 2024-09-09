using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class Numbers404Listener : ISignalRListener
{
	public static EventHandler<string> StartingNumbers;
	public static EventHandler<string> OnGuess;
	public static EventHandler<string> OnVote;
	public static EventHandler<string> OnResult;
	public static EventHandler<string> EndedNumber;
	
	public Numbers404Listener(HubConnection hubConnection)
	{
		hubConnection.On<string>("StartingNumbers", message =>
		{
			StartingNumbers?.Invoke(this, message);
		});
		hubConnection.On<string>("OnGuess", message =>
		{
			OnGuess?.Invoke(this, message);
		});
		hubConnection.On<string>("OnVote", message =>
		{
			OnVote?.Invoke(this, message);
		});
		hubConnection.On<string>("OnResult", message =>
		{
			OnResult?.Invoke(this, message);
		});
		hubConnection.On<string>("EndedNumber", message =>
		{
			EndedNumber?.Invoke(this, message);
		});
	}
}
