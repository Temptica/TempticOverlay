using System;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Spotify;
using TwitcherSharp;
using TwitcherSharp.Api.Generated;
using TwitcherSharp.Api.Generated.Users;
using TwitcherSharp.Chat;
using TwitcherSharp.Extensions;

namespace Temptica.Overlay.Scripts;

public partial class Overlay : Node3D
{
	private static SpotifyService SpotifyService { get; set; }
	public static string BroadcasterId => BroadcastUser.Id; //542726050
	public static TwitchUser BroadcastUser { get; private set; }

	public override async void _Ready()
	{
		try
		{
			var tokens = new AccessTokenService();
			tokens.LoadTokens();
			SpotifyService = new SpotifyService(tokens);

			var test = new Node();
			test.SetTwitcherSharp(TwitchService.Instance);
			test.GetTwitcherSharp<TwitchService>();
			test.RemoveTwitcherSharp();
			test.HasTwitcherSharp();
			
			await TwitchService.Instance.Setup();
			BroadcastUser = (await TwitchApi.Instance.GetUsers()).Data[0];
			
			TwitchChat.CreateInstance(x =>
			{
				x.BroadcasterUser = BroadcastUser;
				x.SenderUser = BroadcastUser;
			});
		
			_ = Task.Run(async () => await SpotifyService.Initialize());
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
	}
}
