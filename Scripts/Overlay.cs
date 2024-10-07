using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.Alerts;
using Temptic404Overlay.Scripts.Services;
using Temptic404Overlay.Scripts.SignalR;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptic404Overlay.Scripts.SignalR.Listeners.Music;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts;

public partial class Overlay : Node3D
{
	public static SignalRService SignalRService => _signalRService;
	public static SpotifyService SpotifyService => _spotifyService;
	private static SignalRService _signalRService;
	private static SpotifyService _spotifyService;
	

	public override void _Ready()
	{
		_signalRService = new SignalRService();
		var tokens = new AccessTokenService();
		
		tokens.LoadTokens();
		_spotifyService = new SpotifyService(tokens);
		_ = Task.Run(async () => await _spotifyService.Initialize());
		VoiceMeeterService.LogIn().Wait();
		SlotMachine.Spin("test", 100);
	}

	public override async void _Notification(int what)
	{
		if (what != NotificationCrash && what != NotificationWMCloseRequest) return;
		await _signalRService.DisposeAsync();
		await VoiceMeeterService.Logout();
	}
}
