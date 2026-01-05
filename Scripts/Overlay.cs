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
	}

	public override void _Notification(int what)
	{
		if (what != NotificationCrash && what != NotificationWMCloseRequest) return;
		SignalRService.DisposeAsync().AsTask().RunSynchronously();
		_webSocketService.Dispose();
	}
}
