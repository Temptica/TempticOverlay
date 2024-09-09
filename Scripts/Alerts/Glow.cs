using Godot;
using System;
using System.Threading.Tasks;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Environment = Godot.Environment;

public partial class Glow : MeshInstance3D
{
	private StandardMaterial3D _material;
	private Environment _environment;
	private Color _lastColor;
	private bool _RainbowMode;
	private bool _paused;
	const double GlowIntensity = 0.75d;
	private Tween _rainbowTween;

	public override void _Ready()
	{
		try
		{
			_material = Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
			_environment = GetNode<Camera3D>("%Camera").Environment;
			_lastColor = _material!.AlbedoColor;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}
		SetColorListener.SetColor += SetColor;
		_rainbowTween = GetTree()
			.CreateTween()
			.SetLoops();
		
		_rainbowTween.TweenProperty(_material, "albedo_color", Colors.Orange, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(_material, "albedo_color", Colors.Yellow, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(_material, "albedo_color", Colors.Green, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(_material, "albedo_color", Colors.LightBlue, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(_material, "albedo_color", Colors.Red, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.Pause();
		
	}

	private void SetColor(object sender, string e)
	{
		SetColor(new Color(e));
	}

	public void SetColor(Color color, float duration = 0.5f)
	{
		try
		{
			_lastColor = color;
			if(_paused || _RainbowMode) return;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(_material, "albedo_color", new Color(_lastColor), duration).SetTrans(Tween.TransitionType.Cubic);
		}
		catch (Exception exception)
		{
			GD.PrintErr(exception);
		}
	}
	public void PlayFlashBang()
	{
		_paused = true;
		
		Task.Run(async () =>
		{
			if (AudioListener.PartyMode)
			{
				AudioListener.PausePartyMode();
				GD.Print("Paused party mode");
			}
			else if (_RainbowMode)
			{
				StopRainbowMode();
			}
			
			await Task.Delay(1000);
			_material.AlbedoColor = Colors.White;
			GD.Print("Full flash");
			_environment.GlowIntensity = 8;
			_environment.GlowStrength = 2;
			
			GD.Print(_environment.GlowIntensity);
			
			var tweenIntensity = GetTree().CreateTween();
			var tweenGlow = GetTree().CreateTween();
			
			tweenIntensity
				.TweenProperty(_environment, "glow_intensity", GlowIntensity, 2)
				.SetTrans(Tween.TransitionType.Cubic);
			
			tweenGlow
				.TweenProperty(_environment, "glow_strength", 1, 2)
				.SetTrans(Tween.TransitionType.Cubic);
			
			
			tweenIntensity.TweenCallback(Callable.From(() =>
			{
				_paused = false;
				SetColor(_lastColor);
				
				if (AudioListener.PartyMode)
				{
					AudioListener.ResumePartyMode();
				}
				else if (_RainbowMode)
				{
					StartRainbowMode();
				}
			}));
			
			
		});
	}
	
	public Color GetColor()
	{
		return _lastColor;
	}
	
	public void StartRainbowMode(float speedScale = 0.25f)
	{
		if(_RainbowMode) return;
		_rainbowTween.Play();
		_rainbowTween.SetSpeedScale(speedScale);
		_RainbowMode = true;
	}
	
	public void StopRainbowMode()
	{
		if(!_RainbowMode) return;
		_rainbowTween.Stop();
		
		_RainbowMode = false;
		SetColor(_lastColor);
	}
}
