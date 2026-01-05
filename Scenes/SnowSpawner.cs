using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptica.Overlay.Scripts.Winter;

namespace Temptica.Overlay.Scenes;

public partial class SnowSpawner : Node3D
{
	public static SnowSpawner Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	private DateTime _nextTime;
	private PackedScene _snowFlake;
	private PackedScene _package;
	private bool _enabled = true;
	private DateTime _nextPackage;
	private readonly List<Package> _packages = [];
	private readonly List<Snow> _snowFlakes = [];
	public override void _Ready()
	{
		Instance = this;
		
		_nextTime = DateTime.Now;
		_nextPackage = DateTime.Now.AddMinutes(0); // Change later
		_snowFlake  = GD.Load<PackedScene>("res://Templates/snow.tscn");
		_package = GD.Load<PackedScene>("res://Templates/package.tscn");
		if (_nextTime.Month is not 12 and not 1 )
		{
			_enabled = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!_enabled) return;
		if (DateTime.Now >= _nextTime)
		{
			_nextTime = DateTime.Now.AddMilliseconds(new Random().Next(750, 1000));
			var flake = (Snow)_snowFlake.Instantiate();
			flake.Hide();
			AddChild(flake);
			
			//random between 0.00 and 16.00
			var randomX = new Random().NextSingle()*16f;
			
			//scale between 0.75 and 1
			var randomScale = new Random().NextSingle() * 0.25f + 0.75f;
			flake.InitialX = randomX;
			flake.GlobalPosition = new Vector3(randomX, 10, 1f);
			flake.Scale = Vector3.One*randomScale;
			flake.Show();
			_snowFlakes.Add(flake);
		}

		return;
		if (DateTime.Now >= _nextPackage)
		{
			_nextPackage = DateTime.Now.AddMinutes(15);
			var package = (Package)_package.Instantiate();
			AddChild(package);
			
			var randomX = new Random().NextSingle()*14f+1;
			var randomY = new Random().NextSingle() * 7f+1;
			package.GlobalPosition = new Vector3(randomX, randomY, 1f);
			_packages.Add(package);
		}
	}

	public static void CheckPackages(Vector2 position, string userName)
	{
		var packages = Instance._packages;
		foreach (var package in packages)
		{
			package.IsHit(position, userName);
		}

		foreach (var snow in Instance._snowFlakes.ToList())
		{
			snow.CheckHit(position);
		}
	}

	public void RemoveSnowFlake(Snow flake)
	{
		_snowFlakes.Remove(flake);
	}
}