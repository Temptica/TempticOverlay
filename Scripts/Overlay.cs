using System;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.SignalR;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts;

public partial class Overlay : Node3D
{
	public static SignalRService SignalRService { get; private set; }

	public static SpotifyService SpotifyService { get; private set; }

	private static WebSocketService _webSocketService;

	public override void _Ready()
	{
		SignalRService = new SignalRService();
		_webSocketService = new WebSocketService();
		
		var tokens = new AccessTokenService();
		
		tokens.LoadTokens();
		SpotifyService = new SpotifyService(tokens);
		_ = Task.Run(async () => await SpotifyService.Initialize());
		_ = Task.Run(async () => await VoiceMeeterService.LogIn());
	}

	public override async void _Notification(int what)
	{
		try
		{
			if (what != NotificationCrash && what != NotificationWMCloseRequest) return;
			await SignalRService.DisposeAsync();
			_webSocketService.Dispose();
			VoiceMeeterService.Logout();
		}
		catch (Exception e)
		{
			throw; //app is closing anyway
		}
	}
}
