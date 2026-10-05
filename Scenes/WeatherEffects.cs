using Godot;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.Weather;

namespace Temptica.Overlay.Scenes;

public partial class WeatherEffects : Node3D
{
	[Export] private GpuParticles3D _rain;
	[Export] private GpuParticles3D _snow;

	public override void _Ready()
	{
		var weatherService = WeatherService.Instance;
		weatherService.WeatherDataReceived += OnWeatherDataReceived;

		Apply(weatherService.LastWeatherData is { } data ? Precipitation.From(data) : Precipitation.None);
	}

	public override void _ExitTree()
	{
		if (WeatherService.Instance is { } weatherService)
			weatherService.WeatherDataReceived -= OnWeatherDataReceived;
	}

	private void OnWeatherDataReceived(WeatherData data) => Apply(Precipitation.From(data));

	private void Apply(Precipitation precipitation)
	{
		SetEmitter(_rain, precipitation.Type == PrecipitationType.Rain, precipitation.Intensity);
		SetEmitter(_snow, precipitation.Type == PrecipitationType.Snow, precipitation.Intensity);
	}

	private static void SetEmitter(GpuParticles3D particles, bool active, float intensity)
	{
		particles.Emitting = active;
		if (active) particles.AmountRatio = intensity;
	}
}
