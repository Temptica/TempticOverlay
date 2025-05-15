using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.Overlay.Scripts.Spotify;

namespace Temptica.Overlay.Scripts.Services;

public class WebSocketService:IDisposable
{
    private readonly HttpListener _listener;
    private bool _isListening;
    
    public WebSocketService()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:8022/overlay/");
        _listener.Start();
        Console.WriteLine("WebSocket server started on ws://localhost:8022/overlay/");
        _isListening = true;
        _ = Listen();
    }

    private async Task Listen()
    {
        while (_isListening)
        {
            var context = await _listener.GetContextAsync();
            
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                Console.WriteLine("Client connected!");

                _ = Task.Run(() => HandleWebSocket(webSocketContext.WebSocket));
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async Task HandleWebSocket(WebSocket webSocket)
    {
        var buffer = new byte[4096];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await OnMessageReceived(message);

        }
    }

    public void Dispose()
    {
        _isListening = false;
        _listener.Stop();
    }


    private static async Task OnMessageReceived(string message)
    {
        switch (message)
        {
            case "SkipSpeech":
                AlertQueue.SkipSpeech();
                break;
            case "Hide":
                Otter.ShowHideOtterEvent.Invoke(null,false);
                break;
            case "Show":
                Otter.ShowHideOtterEvent.Invoke(null,true);
                break;
            case "StopSpotify":
                await SpotifyService.Stop();
                break;
            case "StartSpotify":
                await SpotifyService.Start();
                break;
            case "ZoomIn":
                Otter.ZoomOtterEvent.Invoke(null,true);
                break;
            case "NormalPosition":
                Otter.ZoomOtterEvent.Invoke(null,false);
                break;
                
        }
    }
}