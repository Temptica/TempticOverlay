using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.Overlay.Scripts.Models;
using Temptica.TwitchBot.Shared.enums;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class AlertListener : ISignalRListener
{
	public AlertListener(HubConnection connection)
	{
		connection.On<string, string, AlertType,int>(OverlayHubMethodes.Alert, (user, message, type, amount) =>
		{
			AlertQueue.AddAlert(new OverlayAlert(user, message, type, amount));
		});
		// ReplayLastAlerts (int amount), ResumeAlerts, SkipAllFollows, StopAlerts
		
		connection.On<int>(OverlayHubMethodes.ReplayLastAlerts, AlertQueue.ReplayLastAlerts);
		
		connection.On("ResumeAlerts", AlertQueue.ResumeAlerts);
		
		connection.On("SkipAllFollows", AlertQueue.SkipAllFollows);
		
		connection.On("StopAlerts", AlertQueue.StopAlerts);
	}
}
