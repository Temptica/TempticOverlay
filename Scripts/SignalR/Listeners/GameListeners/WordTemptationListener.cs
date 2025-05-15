using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class WordTemptationListener : ISignalRListener
{
	public static EventHandler<string> StartingWord;
	public static EventHandler<string> WordFound;
	public static EventHandler<string> EndedWord;
	public static EventHandler<string> LettersChanged;
	public WordTemptationListener(HubConnection hubConnection)
	{
		hubConnection.On<string>("StartingWord", message =>
		{
			StartingWord?.Invoke(this, message);
		});
		hubConnection.On<string>("WordFound", message =>
		{
			WordFound?.Invoke(this, message);
		});
		hubConnection.On<string>("EndedWord", message =>
		{
			EndedWord?.Invoke(this, message);
		});
		hubConnection.On<string>("LettersChanged", message =>
		{
			LettersChanged?.Invoke(this, message);
		});
	}
}
