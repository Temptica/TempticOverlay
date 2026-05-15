using System;
using Godot;

using SnowSpawner = Temptica.Overlay.Scenes.SnowSpawner;

namespace Temptica.Overlay.Scripts.Winter;

public partial class Snow : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public float InitialX;
	[Export] public float Amplitude = 0.75f;
	[Export] public float Frequency = 0.25f;
	
	[Export] private float _fallSpeedMin = 0.6f;
	[Export] private float _fallSpeedMax = 0.8f;
	private float _fallSpeed;
	private float _time;
	
	public override void _Ready()
	{
		var random = new Random();
		_fallSpeed = random.NextSingle()*(_fallSpeedMax-_fallSpeedMin)+_fallSpeedMin;
		_time = random.NextSingle()*100;
	}
	
	public override void _Process(double delta)
	{
		_time += (float)delta;
		//move object smootly from left to right
		var yPos = GlobalPosition.Y;
		yPos -= (float)(_fallSpeed * delta);
		var xPos = Mathf.Sin(_time * Frequency) * Amplitude + InitialX;
		
		GlobalPosition = new Vector3(xPos, yPos, GlobalPosition.Z);
		
		if (GlobalPosition.Y < -3)
		{
			SnowSpawner.Instance.RemoveSnowFlake(this);
			QueueFree();
		}
	}

	public void CheckHit(Vector2 click)
	{
        var pos = new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Y);
        
        if (!(pos.DistanceTo(click) < 0.5f)) return;
        
        SnowSpawner.Instance.RemoveSnowFlake(this);
		SnowballSpawner.Instance.SpawnBall();
		QueueFree();
	}
}