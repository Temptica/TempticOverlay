using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Godot;

namespace Temptica.Overlay.Scripts.Services;

public partial class WeatherService : Node
{
    public static WeatherService Instance { get; set; }
    private HttpListener _listener;
    private bool _isListening;

    private const string Port = "8080";

    [Signal]
    public delegate void WeatherDataReceivedEventHandler(WeatherData data);

    public WeatherData LastWeatherData { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        StartServer();
    }

    private void StartServer()
    {
        _listener = new HttpListener();
        // Listen on all local network interfaces for incoming traffic on your port
        _listener.Prefixes.Add($"http://*:{Port}/");

        try
        {
            _listener.Start();
            _isListening = true;
            GD.Print($"[Weather Server] Listening on port {Port}...");

            // Run the listening loop in a background thread so it doesn't freeze Godot
            Task.Run(ListenLoop);
        }
        catch (HttpListenerException ex)
        {
            GD.PrintErr($"[Weather Server] Failed to start server: {ex.Message}");
            GD.PrintErr(
                "Tip: On Windows, you may need to run Godot as Admin or use 'netsh http add urlacl' for non-localhost prefixes.");
        }
    }

    private async Task ListenLoop()
    {
        while (_isListening)
        {
            try
            {
                // Wait asynchronously for the weather station to send data
                var context = await _listener.GetContextAsync();
                var request = context.Request;

                if (request.HttpMethod is "POST" or "GET")
                {
                    // Read the body payload sent by the station
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    var rawData = await reader.ReadToEndAsync();

                    // If the station sends data as Query String format (Wunderground protocol style)
                    // Example: "tempf=68.5&humidity=54&passkey=..."
                    var queryData = HttpUtility.ParseQueryString(rawData);

                    // If empty, it might have sent it via URL query parameters instead (GET request)
                    if (queryData.Count == 0 && !string.IsNullOrEmpty(request.Url?.Query))
                    {
                        queryData = HttpUtility.ParseQueryString(request.Url.Query);
                    }

                    // Extract data (using Ecowitt/Wunderground standard fields)
                    LastWeatherData = new WeatherData(queryData);
                    
                    CallDeferred("emit_signal", "WeatherDataReceived", LastWeatherData);
                }

                // Send a 200 OK response back to the station so it knows we received it
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.OK;
                var buffer = "Success"u8.ToArray();
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                if (!_isListening) break;
                GD.PrintErr($"[Weather Server] Error processing request: {ex.Message}");
            }
        }
    }

    public override void _ExitTree()
    {
        // Clean up when leaving the scene or closing the app
        Instance = null;
        _isListening = false;
        if (_listener is not { IsListening: true }) return;

        _listener.Stop();
        _listener.Close();
        GD.Print("[Weather Server] Server stopped.");
    }
}

public partial class WeatherData(NameValueCollection param) : RefCounted
{
    private const float InchesToMm = 25.4f;
    private const float MilesToKm = 1.609344f;
    private const float KnotsToMps = 0.8689762419006434f;
    private const float InToHpa = 33.86388666666667f;

    public DateTime DateUtc { get; set; } = DateTime.Parse(param["dateutc"]);

    //Indoor
    public float TempInF { get; set; } = float.Parse(param["tempinf"]);
    public float TempInC => MathF.Round((TempInF - 32f) * 5f / 9f,2);
    public float HumidityIn { get; set; } = float.Parse(param["humidityin"]);

    //Outdoor
    public float BaromRelIn { get; set; } = float.Parse(param["baromrelin"]);
    public float BaromRelHpa => MathF.Round(BaromRelIn * InToHpa,2);
    public float TempF { get; set; } = float.Parse(param["tempf"]);
    public float TempC => MathF.Round((TempF - 32f) * 5f / 9f,2);
    public float Humidity { get; set; } = float.Parse(param["humidity"]);
    public float WindDir { get; set; } = float.Parse(param["winddir"]);
    public float WindSpeedMph { get; set; } = float.Parse(param["windspeedmph"]);
    public float WindSpeedKph => MathF.Round(WindSpeedMph * MilesToKm,2);
    public float WindSpeedKts => MathF.Round(WindSpeedMph * KnotsToMps,2);
    public float WindGustMph { get; set; } = float.Parse(param["windgustmph"]);
    public float WindGustKph => MathF.Round(WindGustMph * MilesToKm,2);
    public float WindGustKts => MathF.Round(WindGustMph * KnotsToMps,2);
    public float MaxDailyGust { get; set; } = float.Parse(param["maxdailygust"]);
    public float MaxDailyGustKph => MathF.Round(MaxDailyGust * MilesToKm,2);
    public float MaxDailyGustKts => MathF.Round(MaxDailyGust * KnotsToMps,2);
    public float SolarRadiation { get; set; } = float.Parse(param["solarradiation"]);
    public float Uv { get; set; } = float.Parse(param["uv"]);
    public float RainRateIn { get; set; } = float.Parse(param["rainratein"]);
    public float RainRateMmHour => MathF.Round(RainRateIn * InchesToMm,2);
    public float EventRainIn { get; set; } = float.Parse(param["eventrainin"]);
    public float EventRainMm => MathF.Round(EventRainIn * InchesToMm,2);
    public float HourlyRainIn { get; set; } = float.Parse(param["hourlyrainin"]);
    public float HourlyRainMm => MathF.Round(HourlyRainIn * InchesToMm,2);
    public float DailyRainIn { get; set; } = float.Parse(param["dailyrainin"]);
    public float DailyRainMm => MathF.Round(DailyRainIn * InchesToMm,2);
    public float WeeklyRainIn { get; set; } = float.Parse(param["weeklyrainin"]);
    public float WeeklyRainMm => MathF.Round(WeeklyRainIn * InchesToMm,2);
    public float MonthlyRainIn { get; set; } = float.Parse(param["monthlyrainin"]);
    public float MonthlyRainMm => MathF.Round(MonthlyRainIn * InchesToMm,2);
    public float YearlyRainIn { get; set; } = float.Parse(param["yearlyrainin"]);
    public float YearlyRainMm => MathF.Round(YearlyRainIn * InchesToMm,2);
    public float TotalRainIn { get; set; } = float.Parse(param["totalrainin"]);
    public float TotalRainMm => MathF.Round(TotalRainIn * InchesToMm,2);
}