using Godot;
using Temptica.Overlay.Scripts.Services;

namespace Temptica.Overlay.Scripts.Weather;

public enum PrecipitationType
{
    None,
    Rain,
    Snow
}

public readonly record struct Precipitation(PrecipitationType Type, float Intensity)
{
    private const float SnowMaxTempC = 1f;
    private const float MinIntensity = 0.2f;
    private const float HeavyRainRateMmHour = 10f;

    public static Precipitation None => new(PrecipitationType.None, 0f);

    public static Precipitation From(WeatherData data)
    {
        if (data.RainRateMmHour <= 0f) return None;

        var type = data.TempC <= SnowMaxTempC ? PrecipitationType.Snow : PrecipitationType.Rain;
        var intensity = Mathf.Clamp(data.RainRateMmHour / HeavyRainRateMmHour, MinIntensity, 1f);

        return new Precipitation(type, intensity);
    }
}
