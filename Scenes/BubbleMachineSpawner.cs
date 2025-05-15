using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;
using BubbleSpawner = Temptica.Overlay.Scripts.Spawners.BubbleSpawner;

namespace Temptica.Overlay.scenes;

public partial class BubbleMachineSpawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private PackedScene _bubble;
	[Export]
	private BubbleMachineSpawner _bubbleMachineSpawner;
	private float _timer;
	[Export]
	private double _spawnTime = 0.300f;
	private double _remainingTimeTillNext;
	

	public override void _Ready()
	{
		_bubble = GD.Load<PackedScene>("res://Templates/bubble.tscn");
		_remainingTimeTillNext = _spawnTime;
		SpawnersListener.OnSpawnBubble += (_,_) => AddBubbleTime(5);
	}

	private void AddBubbleTime(float bubbleTime)
	{
		_timer += bubbleTime;
	}

	public override void _Process(double delta)
	{
		if (_timer > 0)
		{
			_remainingTimeTillNext -= delta;
			
			if (_remainingTimeTillNext <= 0)
			{
				SpawnBubble();
				_remainingTimeTillNext = _spawnTime;
			}
			
			_timer -= (float)delta;
		}
	}

	private void SpawnBubble()
	{
		BubbleSpawner.SpawnBubble.Invoke(this, null);
	}
}