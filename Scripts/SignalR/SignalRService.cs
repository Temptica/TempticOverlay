using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
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
	private int _retryCounter;
	
	public SignalRService()
	{
		try
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
		catch (Exception e)
		{
			GD.Print(e);
		}
	}

	private async Task StartAsync()
	{
		try
		{
			if (_isConnected)
			{
				GD.Print("Connected");
				_retryCounter = 0;
				return;
			}

			await _overlayHubConnection.StartAsync();
			_isConnected = _overlayHubConnection.State == HubConnectionState.Connected;
			GD.Print("Connected");
		}
		catch (HttpRequestException)
		{
			_retryCounter++;
			if (_retryCounter >= 10)
			{
				GD.Print("Failed to connect to SignalR");
				return;
			}
			await Task.Delay(_retryCounter * 5000);
			await StartAsync();
		}
		catch (Exception e)
		{
			GD.PushError(e);
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
	
	public void SendChatMessage(string message)
	{
		_overlayHubConnection.InvokeAsync("SendChatMessage", message);
	}

	public void DuelWinnerColor(FishColor winnerFish)
	{
		_overlayHubConnection.InvokeAsync("DuelWinnerColor", (int)winnerFish);
	}

	public async Task RequestAdTime()
	{
		await _overlayHubConnection.InvokeAsync("NextAdRequest");
	}

	public void TrashClicks(string username, int points)
	{
		_overlayHubConnection.InvokeAsync("TrashClicked", username, points);
	}
}
