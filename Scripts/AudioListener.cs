using System;
using System.Linq;
using Godot;
using Temptica.Overlay.Scripts.Alerts;
using Temptica.Overlay.Scripts.Labels;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.SignalR.Listeners.Music;
using Environment = Godot.Environment;

namespace Temptica.Overlay.Scripts;

public partial class AudioListener : Node3D
{
	private static Environment _environment;
	private static Glow _glow;
	private static PartyModeLabel _partyModeLabel;
	public static bool PartyMode { get; private set; }
	private static bool _paused;
	private int _busIndex;
	
	private const float MinDb = 80.0f;
	private float _maxEnergy;
	private const float MinGlow = 1f;
	private const float MaxGlow = 1.35f;
	private float _currentPeak;
	private double _timeSinceLastPeak;
	private const float EnergyResetTime = 3f;
	
	public static float Energy { get; private set; }
	public override void _Ready()
	{
		var input = AudioServer.GetInputDeviceList().First(o => o.Contains("CABLE-A"));
		AudioServer.InputDevice = input;
		var output = AudioServer.GetOutputDeviceList().First(o => o.Contains("VAIO3"));
		
		AudioServer.OutputDevice = output;
		
		_busIndex = AudioServer.GetBusIndex("Music");
		_environment = GetNode<Camera3D>("%Camera").Environment;
		_glow = GetNode<Glow>("%Glow");
	   
		StartPartyModeListener.StartPartyMode += (_, _) => StartPartyMode();
		StopPartyModeListener.StopPartyMode += (_, _) => EndPartyMode();
		_partyModeLabel = GetNode<PartyModeLabel>("%PartyModeLabel");
	}

	public override void _Process(double delta)
	{
		var spectrum = (AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(_busIndex, 0);
		
		var magnitude = ((Vector2)spectrum.Call("get_magnitude_for_frequency_range", 100, 500)).Length();
		
		var db = (MinDb + Mathf.LinearToDb(magnitude)) / MinDb;
		
		Energy = Mathf.Clamp(db , 0 , 1f);
			
		const float threshold = 0.55f;
		Energy = (float)Math.Round(Energy, 3);
		_maxEnergy = Mathf.Max(_maxEnergy, Energy);
			
		if(Energy > threshold)
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
		if(!PartyMode || _paused) return;
		var glow = Mathf.Lerp(MinGlow, MaxGlow, Energy);
		Energy = glow;
		_environment.GlowStrength = Mathf.Lerp(_environment.GlowStrength, glow, Math.Abs(_environment.GlowStrength - glow));
	}
	
	public void StartPartyMode()
	{
		PartyMode = true;
		_paused = false;
		_glow.StartRainbowMode();
		_partyModeLabel.StartPartyMode();
		
		//audio bus: set recordStopper to Unmute
		var idx = AudioServer.GetBusIndex("recordStopper");
		AudioServer.SetBusMute(idx, false);
		VoiceMeeterService.MuteSpotify();
	}
	public static void PausePartyMode()
	{
		_glow.StopRainbowMode();
		_partyModeLabel.EndPartyMode();
		_partyModeLabel.Hide();
		_environment.CallDeferred("set_glow_strength", MinGlow);
		_paused = true;
	}
	
	public static void ResumePartyMode()
	{
		_glow.StartRainbowMode();
		_partyModeLabel.StartPartyMode();
		_partyModeLabel.Show();
		_paused = false;
	}
	
	
	
	public void EndPartyMode()
	{
		PartyMode = false;
		_glow.StopRainbowMode();
		//audio bus: set recordStopper to mute
		var idx = AudioServer.GetBusIndex("recordStopper");
		AudioServer.SetBusMute(idx, true);
		VoiceMeeterService.UnmuteSpotify();
		_partyModeLabel.EndPartyMode();
		_environment.CallDeferred("set_glow_strength", 1f);
	}
}