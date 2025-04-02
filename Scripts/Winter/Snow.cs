using Godot;
using System;

public partial class Snow : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public float InitialX;
	[Export] public float Amplitude = 0.75f;
	[Export] public float Frequency = 0.25f;
	
	[Export] private float _fallSpeedMin = 0.95f;
	[Export] private float _fallSpeedMax = 1f;
	private float _fallSpeed;
	private float _time;
	
	public override void _Ready()
	{
		_fallSpeed = new Random().Next((int)_fallSpeed*100, (int)_fallSpeedMax*100)/100f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
	
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
			QueueFree();
		}
	}
}
