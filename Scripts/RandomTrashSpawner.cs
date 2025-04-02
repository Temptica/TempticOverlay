using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Temptic404Overlay.Scripts;

public partial class RandomTrashSpawner : Node3D
{
	private PackedScene _trash;

	[Export] public int MinTimeTillNextSpawn = 60;
	[Export] public int MaxTimeTillNextSpawn = 240;
	[Export] private Node3D _leftBound;
	[Export] private Node3D _rightBound;
	
	public static List<Trash> Trashes = new();

	private double remainingTime;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_trash = (PackedScene)ResourceLoader.Load("res://Templates/trash.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		remainingTime -= delta;

		Trashes = GetChildren().Cast<Trash>().ToList();

		if (remainingTime <= 0)
		{
			GD.Print("RandomTrashSpawner");
			var trash = _trash.Instantiate<Trash>();
			AddChild(trash);
			trash.InitialX = new Random().Next((int)_leftBound.GlobalPosition.X, (int)_rightBound.GlobalPosition.X);
			trash.GlobalPosition = new Vector3(trash.InitialX, trash.GlobalPosition.Y, trash.GlobalPosition.Z);
			remainingTime = new Random().Next(MinTimeTillNextSpawn, MaxTimeTillNextSpawn);
			
		}
	}

	public static bool CheckTrashHit(Vector2 position, string username, out int points)
	{
		GD.Print("Checking for hits");
		var clickedTrash = Trashes.Where(trash => trash.IsHit(position)).ToList();
		GD.Print($"Found {clickedTrash.Count} fishes");
		ClickCounterDisplay.UpdateFishes(clickedTrash.Count);
		points = clickedTrash.Count;

		if (points <= 0) return false;
		
		GD.Print($"clicked for {points} points");
		
		clickedTrash.ForEach(f=>f.QueueFree());
		Trashes.RemoveAll(fish => clickedTrash.Contains(fish));
		Overlay.SignalRService.TrashClicks(username, points);
		return true;
	}
}
