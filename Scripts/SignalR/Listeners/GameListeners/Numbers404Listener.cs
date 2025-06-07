using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class Numbers404Listener : ISignalRListener
{
	public static EventHandler<string> StartingNumbers;
	public static EventHandler<string> OnGuess;
	public static EventHandler<string> OnVote;
	public static EventHandler<string> OnResult;
	public static EventHandler<string> EndedNumber;
	
	public Numbers404Listener(HubConnection hubConnection)
	{
		hubConnection.On<string>(OverlayHubMethodes.StartNumbers404, message =>
		{
			StartingNumbers?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.SendGuess, message =>
		{
			OnGuess?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.StartNumberVote, message =>
		{
			OnVote?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.SendNumberResult, message =>
		{
			OnResult?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.EndNumbers, message =>
		{
			EndedNumber?.Invoke(this, message);
		});
	}
}
