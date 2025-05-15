using System;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptica.Overlay.Templates;

namespace Temptica.Overlay.Scripts.Winter;

public partial class SnowballSpawner : Node3D
{
	[Export] public Node3D Target;
	[Export] private double _spawnRate;
	private double _timeTillNextSpawn;
	private PackedScene _snowBallScene;

	private int _ballsToSpawn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_snowBallScene = GD.Load<PackedScene>("res://Templates/snow_ball.tscn");

		SnowBallListener.SpawnSnowBall += (_, _) =>
		{
			_ballsToSpawn++;
		};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_timeTillNextSpawn += delta;

		if (_spawnRate < _timeTillNextSpawn && _ballsToSpawn>0)
		{
			var snowBall = _snowBallScene.Instantiate<SnowBall>();
			AddChild(snowBall);

			var height = new Random().Next(75, 300) / 100f;

			var targetHeight = new Vector3(Target.GlobalPosition.X, (float)new Random().NextDouble()*2f+Target.GlobalPosition.Y, Target.GlobalPosition.Z);
			var velocity = CalculateShotVelocity(snowBall.GlobalPosition, targetHeight, height);
			snowBall.LinearVelocity = velocity;
			_timeTillNextSpawn = 0;
			_ballsToSpawn--;
		}
	}

	private static Vector3 CalculateShotVelocity(Vector3 from, Vector3 to, float additionalHeight) {

		var maxHeight = Mathf.Max(from.Y, to.Y) + additionalHeight;
		var verticalVelocity = Mathf.Sqrt((maxHeight - from.Y) * 2 * 9.8f);

		var peakTime = verticalVelocity / 9.8f;
		var fallTime = Mathf.Sqrt((maxHeight - to.Y) * 2 / 9.8f);
		var totalTime = peakTime + fallTime;

		var positionDelta = to - from;
		positionDelta.Y = 0;

		var distance = positionDelta.Length();

		var finalVelocity = positionDelta.Normalized() * (distance / totalTime);
		finalVelocity.Y = verticalVelocity;

		return finalVelocity;
	}
}