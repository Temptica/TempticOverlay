using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptic404Overlay.Scripts.Alerts;
using Temptic404Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.SignalR.Listeners;

public class AlertListener : ISignalRListener
{
	
	public AlertListener(HubConnection connection)
	{
		connection.On<string, string, AlertType,int>("Alert", (user, message, type, amount) =>
		{
			AlertQueue.AddAlert(new OverlayAlert(user, message, type, amount));
			GD.Print($"Alert: {user} {message} {type} {amount}");
		});
		// ReplayLastAlerts (int amount), ResumeAlerts, SkipAllFollows, StopAlerts
		
		connection.On<int>("ReplayLastAlerts", (count) =>
		{
			AlertQueue.ReplayLastAlerts(count);
			GD.Print($"Replay Last Alerts: {count}");
		});
		
		connection.On("ResumeAlerts", () =>
		{
			AlertQueue.ResumeAlerts();
			GD.Print("Resume Alerts");
		});
		
		connection.On("SkipAllFollows", () =>
		{
			AlertQueue.SkipAllFollows();
			GD.Print("Skip All Follows");
		});
		
		connection.On("StopAlerts", () =>
		{
			AlertQueue.StopAlerts();
			GD.Print("Stop Alerts");
		});
	}
}
