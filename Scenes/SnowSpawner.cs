using System;
using Godot;
using Temptica.Overlay.Scripts.Winter;

namespace Temptica.Overlay.scenes;

public partial class SnowSpawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private DateTime _nextTime;
	private PackedScene _snowFlake;
	[Export] private bool _enabled = true;
	public override void _Ready()
	{
		_nextTime = DateTime.Now;
		_snowFlake  = GD.Load<PackedScene>("res://Templates/snow.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!_enabled) return;
		if (DateTime.Now >= _nextTime)
		{
			_nextTime = DateTime.Now.AddMilliseconds(new Random().Next(750, 1000));
			var flake = (Snow)_snowFlake.Instantiate();
			AddChild(flake);
			//random between 0.00 and 16.00
			var randomX = new Random().Next(0,1601)/100f;
			
			//scale between 0.75 and 1
			var randomScale = new Random().Next(75,100)/100f;
			flake.InitialX = randomX;
			flake.GlobalPosition = new Vector3(randomX, 10, 1f);
			flake.Scale = Vector3.One*randomScale;
			
		}
	}
}