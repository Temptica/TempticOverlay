using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptic404Overlay.Scripts.Services;
using Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

namespace Temptic404Overlay.Scripts.Fishes;

public partial class FishSpawner : Node3D
{
	private PackedScene _fishScene;
	private float _timeToSpawn = 2f;
	private double _timeElapsed ;
	private static double _fishesToSpawn ;
	public static readonly List<Fish> Fishes = new();

	public override void _Ready()
	{
		_fishScene = GD.Load<PackedScene>("res://Templates/fish.tscn");
		StartFishGameListener.StartFishGame += (_, _) => StartFishGame() ;
		SpawnFishListener.SpawnFish += (_, fishes) =>
		{
			for (var i = 0; i < fishes; i++)
			{
				SpawnFish();
			}
		};
	}

	private static void StartFishGame()
	{
		_fishesToSpawn += 30;
	}

	public static void SpawnFishes(int count)
	{
		_fishesToSpawn += count;
	}

	public override void _Process(double delta)
	{
		if (_fishesToSpawn <= 0) return;
		if(_timeElapsed >= _timeToSpawn)
		{
			SpawnFish();
			_fishesToSpawn--;
			_timeElapsed = 0;
			return;
		}
		_timeElapsed += delta;
	}

	private void SpawnFish()
	{
		var fish = (Fish)_fishScene.Instantiate();
		CallDeferred(Node.MethodName.AddChild, fish);
		//set global pos as deferred to avoid threading issues
		fish.CallDeferred("set_global_position", new Vector3((float)new Random().NextDouble() * 14 + 1, (float)new Random().NextDouble() * 7 + 1,0));
		Fishes.Add(fish);
	}
	
	public static bool CheckFishesHit(Vector2 position,string username, out int points)
	{
		//check if the position is within the fish
		GD.Print("Checking for hits");
		var clickedFishes =  Fishes.Where(fish => fish.IsHit(position)).ToList();
		GD.Print($"Found {clickedFishes.Count} fishes");
		ClickCounterDisplay.UpdateFishes(clickedFishes.Count);
		points = clickedFishes.Sum(fish => fish.Type switch
		{
			FishType.Normal => 1,
			FishType.Gold => 2,
			FishType.Rainbow => 5,
			_ => 0
		});

		if (points <= 0) return false;
		
		GD.Print($"clicked for {points} points");
		
		clickedFishes.ForEach(f=>f.QueueFree());
		Fishes.RemoveAll(fish => clickedFishes.Contains(fish));
		Overlay.SignalRService.FishClicked(username, points);
		return true;
	}
	
	
}
