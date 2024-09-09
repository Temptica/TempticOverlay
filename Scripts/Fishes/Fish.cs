using System;
using Godot;

namespace Temptic404Overlay.Scripts.Fishes;

public enum FishType
{
	Normal,
	Gold,
	Rainbow
}
public partial class Fish : RigidBody3D
{
	public FishType Type { get; set; }
	private int _hitTimes = 0;
	private Sprite3D _normalFish;
	private Sprite3D _goldFish;
	private AnimatedSprite3D _rainbowFish;
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
		
		//depending on teh type, a different mesh will be loaded
		switch (Type)
		{
			case FishType.Normal:
				_normalFish.Show();
				break;
			case FishType.Gold:
				_goldFish.Show();
				//make it move in a random direction
				LinearVelocity = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), 0);
				break;
			case FishType.Rainbow:
				
				_rainbowFish.Show();
				_rainbowFish.Play("gif");
				LinearVelocity = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), 0);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		BodyEntered += OnBodyEntered;
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
					BubbleSpawner.SpawnBubble?.Invoke(this, GlobalTransform.Origin);
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

	public override void _PhysicsProcess(double delta)
	{
		//a rainbow fish and golden fish should have a speed of at least 0.3f on either X or Y axis
		if (Type is FishType.Normal) return;
		
		if(LinearVelocity.Length() < 0.3f)
		{
			//Always go in teh same direction the fish is already going so it doesn't weirdly turn backwards
			var x = LinearVelocity.X;
			var y = LinearVelocity.Y;

			if (x >= 0)
			{
				x += 0.1f * (float)delta;
			}
			else
			{
				x -= 0.1f * (float)delta;
			}
			
			if(y>=0)
			{
				y += 0.1f * (float)delta;
			}
			else
			{
				y -= 0.1f * (float)delta;
			}
			
			LinearVelocity = new Vector3(x, y, 0);
		}
		
		
	}
	
	
	
	
}
