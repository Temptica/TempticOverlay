using System;
using Godot;

namespace Temptica.Overlay.Scripts.Fishes;

public enum FishType
{
	Normal,
	Gold,
	Rainbow
}
public partial class Fish : RigidBody3D
{
	public FishType Type { get; set; }
	private int _hitTimes;
	private Sprite3D _normalFish;
	private Sprite3D _goldFish;
	private AnimatedSprite3D _rainbowFish;
	private Vector3 _initialVelocity; //Velocity should not be slower, but can change in direction
	private DateTime _timeTillChange;
	public override void _Ready()
	{
		_normalFish = GetChild<Sprite3D>(0);
		_goldFish = GetChild<Sprite3D>(1);
		_rainbowFish = GetChild<AnimatedSprite3D>(2);
		
		var random = new Random();
		var type = random.Next(0, 30);

		Type = type switch
		{
			0 => FishType.Rainbow,
			< 5 => FishType.Gold,
			_ => FishType.Normal
		};
		
		//depending on the type, a different mesh will be loaded
		switch (Type)
		{
			case FishType.Normal:
				_normalFish.Show();
				break;
			case FishType.Gold:
				_goldFish.Show();
				//make it move in a random direction
				LinearVelocity = new Vector3((float)random.NextDouble()-0.5f, (float)random.NextDouble()-0.5f, 0);
				break;
			case FishType.Rainbow:
				
				_rainbowFish.Show();
				_rainbowFish.Play("gif");
				LinearVelocity = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), 0);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		_initialVelocity = LinearVelocity;
		
		BodyEntered += OnBodyEntered;
		
		_timeTillChange += TimeSpan.FromMinutes(5);
	}

	public override void _Process(double delta)
	{
		if (_timeTillChange > DateTime.Now)
		{
			switch (Type)
			{
				case FishType.Normal:
					Type++;
					_normalFish.Hide();
					_goldFish.Show();
					break;
				case FishType.Gold:
					Type++;
					_goldFish.Hide();
					_rainbowFish.Show();
					break;
				case FishType.Rainbow:
					QueueFree();
					break;
			}
			_timeTillChange -= TimeSpan.FromMinutes(5);
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is RigidBody3D collided)
		{
			if (collided is Fish || collided.GetCollisionLayerValue(0))
			{
				//10% chance to spawn a bubble
				if (new Random().Next(0, 10) == 0)
				{
					Spawners.BubbleSpawner.SpawnBubble?.Invoke(this, GlobalTransform.Origin);
				}
			}
		}
	}

	public bool IsHit(Vector2 position)
	{
		var pos = new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Y);
		var isHit = position.DistanceTo(pos) <= 0.4f;
		if(Type == FishType.Rainbow && isHit)
		{
			_hitTimes++;
			
			var currentSize = _rainbowFish.CallDeferred("get_pixel_size").AsDouble();
			_rainbowFish.CallDeferred("set_pixel_size", currentSize-0.001);
			//return _hitTimes >= 3;
		}
		
		return isHit;
	}
}
