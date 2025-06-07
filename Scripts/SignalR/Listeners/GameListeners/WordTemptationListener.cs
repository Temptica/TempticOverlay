using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class WordTemptationListener : ISignalRListener
{
	public static EventHandler<string> StartingWord;
	public static EventHandler<string> WordFound;
	public static EventHandler<string> EndedWord;
	public static EventHandler<string> LettersChanged;
	public WordTemptationListener(HubConnection hubConnection)
	{
		hubConnection.On<string>(OverlayHubMethodes.StartWordTemptation, message =>
		{
			StartingWord?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.WordFound, message =>
		{
			WordFound?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.EndWordTemptation, message =>
		{
			EndedWord?.Invoke(this, message);
		});
		hubConnection.On<string>(OverlayHubMethodes.ChangeLetter, message =>
		{
			LettersChanged?.Invoke(this, message);
		});
	}
}
