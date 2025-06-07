using System;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.TwitchBot.Shared.HubMethodes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners;

public class AdsListener : ISignalRListener
{
        //AdStarted and AdEnded
        
        public static EventHandler AdStarted;
        public static EventHandler<DateTime> AdEnded;

        public AdsListener(HubConnection connection)
        {
                connection.On("AdStarted", () =>
                {
                        AdStarted?.Invoke(this, EventArgs.Empty);
                });
                connection.On<DateTime>(OverlayHubMethodes.AdEnded, nextAd =>
                {
                        AdEnded?.Invoke(this, nextAd);
                });
        }
}