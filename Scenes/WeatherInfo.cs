using Godot;
using Temptica.Overlay.Scripts.Services;

namespace Temptica.Overlay.Scenes;

public partial class WeatherInfo : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private Label3D _weatherLabel;
	
	public override void _Ready()
	{
		_weatherLabel = GetNode<Label3D>("WeatherLabel");
		
		WeatherService.Instance.WeatherDataReceived += OnWeatherDataReceived;
	}

	private void OnWeatherDataReceived(WeatherData data)
	{
		var text = "";
		
		text += $"Temp Ind: {data.TempInC}°C / {data.TempInF}°F | Hum: {data.HumidityIn}%\n";
		text += $"Temp Out: {data.TempC}°C / {data.TempF}°F | Hum: {data.Humidity}%\n";
		text += $"Wind: {data.WindSpeedKph}km/h (Gust {data.WindGustKph}) @ {data.WindDir}° ({data.WindSpeedMph}mph, gust {data.WindGustMph}mph)";
		
		_weatherLabel.Text = text;
	}
}