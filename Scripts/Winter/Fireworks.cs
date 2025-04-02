using Godot;
using System;
using System.Threading.Tasks;
using Temptic404Overlay.Scripts;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

public partial class Fireworks : Node3D
{
	private const double AnimationTime = 3;

	private double _remainingDelta = 0;
	
	[Export]
	private GpuParticles3D _rocketParticles;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FireworkListener.DisplayFirework += (sender, args) =>
		{
			_remainingDelta += AnimationTime;
			_ = Task.Run(async() =>
			{
				await Task.Delay(4000);
				AudioPlayer.PlayAudio(AudioEffects.Pop+1);
			});
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_remainingDelta > 0)
		{
			_rocketParticles.Emitting = true;
			
			_remainingDelta -= delta;
			
		}
		else
		{
			_rocketParticles.Emitting = false;
		}
		_rocketParticles.Emitting = _remainingDelta > 0;
	}
	
	
}
