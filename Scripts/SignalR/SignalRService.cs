using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService : IAsyncDisposable
{
	//private const string hubUrl = "http://192.168.1.12:5000";
	private const string hubUrl = "https://localhost:7299";
	private readonly HubConnection _overlayHubConnection;
	private bool _isConnected;
	private List<ISignalRListener> _listeners;
	
	public SignalRService()
	{
		_overlayHubConnection = new HubConnectionBuilder()
			.WithUrl(hubUrl+"/OverlayHub")
			.Build();
			GD.Print(hubUrl);
		_overlayHubConnection.Closed += async (error) =>
		{
			GD.Print("Connection closed");
			_isConnected = false;
			await Task.Delay(1000);
			await StartAsync();
		};
		
		_listeners = SignalRReflection.RegisterListeners(_overlayHubConnection);

		_ = StartAsync();
	}

	private async Task StartAsync()
	{
		try
		{
			if (_isConnected)
			{
				GD.Print("Connected");
				return;
			}
			await _overlayHubConnection.StartAsync();
			_isConnected = _overlayHubConnection.State == HubConnectionState.Connected;
		}
		catch (Exception e)
		{
			GD.Print(e);
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (_overlayHubConnection != null) await _overlayHubConnection.DisposeAsync();
	}

	public void AddPoints(string username, int sum)
	{
		_overlayHubConnection.InvokeAsync("FishClicked", username, sum);
	}

	public void DuelWinnerColor(FishColor winnerFish)
	{
		_overlayHubConnection.InvokeAsync("DuelWinnerColor", (int)winnerFish);
	}
}
