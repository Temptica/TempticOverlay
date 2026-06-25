using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Godot;

// ReSharper disable once CheckNamespace
namespace Temptica.GreenHeatSharp;

public partial class GreenHeatSharp : Node
{
    public static GreenHeatSharp Instance { get; private set; }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Export] public string StreamName { get; set; }
    [Export] public bool Minify { get; set; }

    /// <summary>
    /// Whether to print debug messages.
    /// </summary>
    [Export]
    public bool Debug { get; set; }

    /// <summary>
    /// Whether to send the clicks as a gd InputEventMouseButton. The data can be found in the meta field. <see cref="Node.GetMeta"/>
    /// </summary>
    [Export]
    public bool SendInputEvent { get; set; }

    [Export] public bool ConnectOnStart { get; set; }

    private Viewport _viewport;

    private const string WebSocketUrl = "wss://heat.prod.kr/";

    [Signal]
    public delegate void MessageReceivedEventHandler(GreenHeatMessage message);

    public bool Started { get; private set; }

    private ClientWebSocket _client;
    private CancellationTokenSource _cancellationTokenSource;

    public override void _Ready()
    {
        _viewport = GetViewport();
        _cancellationTokenSource = new CancellationTokenSource();

        if (ConnectOnStart)
        {
            _ = Connect();
        }
    }

    public async Task Connect()
    {
        var uri = new Uri(WebSocketUrl + StreamName + (Minify ? "?minify" : ""));
        _client = new ClientWebSocket();
        PrintDebug("Connecting to:", uri);
        try
        {
            await _client.ConnectAsync(uri, CancellationToken.None);
            PrintDebug("Connected!");
        }
        catch (Exception e)
        {
            PrintDebug("Error connecting to websocket:", e);
            return;
        }

        Started = true;
        _ = Process();
    }

    private async Task Process()
    {
        var bytes = new byte[1024];
        while (Started)
        {
            var result = await _client.ReceiveAsync(new ArraySegment<byte>(bytes), _cancellationTokenSource.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Started = false;
                await Connect();
            }

            var message = System.Text.Encoding.UTF8.GetString(bytes, 0, result.Count);

            PrintDebug("Received message:", message);
            try
            {
                var data = JsonSerializer.Deserialize<GreenHeatMessage>(message, SerializerOptions);
                
                EmitSignalMessageReceived(data);

                if (!SendInputEvent) continue;

                var inputEvent = new InputEventMouseButton();
                inputEvent.Position = new Vector2(data.X, data.Y);
                inputEvent.Pressed = data.Type == GreenHeatMessageType.Click;
                inputEvent.ButtonIndex = data.Button switch
                {
                    GreenHeatMouseButtonType.Left => MouseButton.Left,
                    GreenHeatMouseButtonType.Middle => MouseButton.Middle,
                    _ => MouseButton.Right
                };
                inputEvent.SetMeta("greenheat", data);
                _viewport.PushInput(inputEvent);
            }
            catch (Exception e)
            {
                PrintDebug("Error processing message:", e);
            }
        }
    }

    public async Task Stop()
    {
        try
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            PrintDebug("Error closing websocket:", e);
        }
        finally
        {
            _client.Dispose();
            await _cancellationTokenSource.CancelAsync();
        }
    }

    public override void _EnterTree()
    {
        Instance = this;
        _cancellationTokenSource?.TryReset();
    }

    public override async void _ExitTree()
    {
        await Stop();
        await _cancellationTokenSource.CancelAsync();
        Instance = null;
    }

    private static void PrintDebug(params object[] args)
    {
        if (!Instance.Debug) return;
        GD.PrintRich(["[color=green][GreenHeatSharp] ", ..args, "[/color]"]);
    }
}

public partial class GreenHeatMessage : RefCounted
{
    /// <summary>
    /// The twitch id (or an opaque id if permission was not given) of the person.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Boolean for if the twitch id is from a user that you can fetch names from. Is null when 'minify' is true.
    /// </summary>
    [JsonPropertyName("is_anonymous")]
    public bool? IsAnonymous { get; set; }

    /// <summary>
    /// Type: the type of the message. Could be "click", "drag", "hover" or "release".
    /// </summary>
    public GreenHeatMessageType Type { get; set; }

    /// <summary>
    /// The X position normalized from 0 to 1. It is made this way to be compatible with legacy heat's schema.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// The Y position normalized from 0 to 1. It is made this way to be compatible with legacy heat's schema.
    /// </summary>
    public float Y { get; set; }

    public GreenHeatMouseButtonType Button { get; set; } = GreenHeatMouseButtonType.Left;

    /// <summary>
    /// Whether 'shift' is used while clicking.
    /// </summary>
    public bool Shift { get; set; }

    /// <summary>
    /// Whether 'ctrl' is used while clicking.
    /// </summary>
    public bool Ctrl { get; set; }

    /// <summary>
    /// Whether 'alt' is used while clicking.
    /// </summary>
    public bool Alt { get; set; }

    /// <summary>
    /// Time since epoch in milliseconds when the message was formed.
    /// </summary>
    public ulong Time { get; set; }

    /// <summary>
    /// The stream latency in seconds between source and viewer. ONLY ACCOUNTS ONE WAY! You might have to figure out your delay to twitch and add it to the total latency.
    /// </summary>
    public float Latency { get; set; }

    /// <summary>
    /// The latency in milliseconds. (same unit as time field). Is null when listening is minify
    /// </summary>
    [JsonPropertyName("latency_ms")]
    public int? LatencyMs { get; set; }

    /// <summary>
    /// Whether the viewer is on a mobile device.
    /// </summary>
    public bool Mobile { get; set; }

    public bool IsClick() => Type == GreenHeatMessageType.Click;
    public bool IsDrag() => Type == GreenHeatMessageType.Drag;
    public bool IsHover() => Type == GreenHeatMessageType.Hover;
    public bool IsRelease() => Type == GreenHeatMessageType.Release;

    public static GreenHeatMessage FromVariant(Variant data) => data.As<GreenHeatMessage>();
}

public enum GreenHeatMessageType
{
    Click,
    Drag,
    Hover,
    Release
}

public enum GreenHeatMouseButtonType
{
    Left,
    Middle,
    Right
}