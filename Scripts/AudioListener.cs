using System;
using Godot;

namespace Temptica.Overlay.Scripts;

public partial class AudioListener : Node3D
{
    private int _busIndex;

    private const float MinDb = 80.0f;
    private float _maxEnergy;
    private const float MinGlow = 1f;
    private const float MaxGlow = 1.35f;
    private float _currentPeak;
    private double _timeSinceLastPeak;
    private const float EnergyResetTime = 3f;
    private AudioEffectSpectrumAnalyzerInstance _spectrumAnalyzer;

    public static float Energy { get; private set; }

    public override void _Ready()
    {
        _busIndex = AudioServer.GetBusIndex("Music");
        _spectrumAnalyzer = (AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(_busIndex, 0);
    }

    public override void _Process(double delta)
    {
        var magnitude = ((Vector2)_spectrumAnalyzer.Call("get_magnitude_for_frequency_range", 100, 500)).Length();
        var db = (MinDb + Mathf.LinearToDb(magnitude)) / MinDb;

        Energy = Mathf.Clamp(db, 0, 1f);

        const float threshold = 0.55f;
        Energy = (float)Math.Round(Energy, 3);
        _maxEnergy = Mathf.Max(_maxEnergy, Energy);

        if (Energy > threshold)
        {
            //energy will be between tresold and 1. It should now be between 0 and 1 (treshold becomes 0,_maxEnergy becomes 1)
            Energy = (Energy - threshold) / (_maxEnergy - threshold);
            _currentPeak = Mathf.Max(_currentPeak, Energy);
        }
        else
        {
            Energy = 0;
            if (_currentPeak > 0)
            {
                _currentPeak = 0;
                _timeSinceLastPeak = 0;
            }
            else if (_timeSinceLastPeak < EnergyResetTime)
            {
                _timeSinceLastPeak += delta;
            }
            else
            {
                _maxEnergy = 0;
            }
        }

        //map energy to glow
        if (!OverlayColorManager.Instance.PartyModeEnabled) return;
        var glow = Mathf.Lerp(MinGlow, MaxGlow, Energy);
        Energy = glow;

        var glowStrength = OverlayColorManager.Instance.GetGlowStrength();

        OverlayColorManager.Instance.SetGlowStrength(Mathf.Lerp(glowStrength, glow, Math.Abs(glowStrength - glow)));
    }

    public static void PausePartyMode()
    {
        OverlayColorManager.Instance.PausePartyMode();
        OverlayColorManager.Instance.SetGlowStrength(MinGlow);
    }

    public static void ResumePartyMode()
    {
        OverlayColorManager.Instance.ResumePartyMode();
    }

    public void EndPartyMode()
    {
        OverlayColorManager.Instance.PausePartyMode();
        OverlayColorManager.Instance.SetGlowStrength(1f);
    }
}