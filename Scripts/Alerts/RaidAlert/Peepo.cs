using System;
using Godot;

namespace Temptica.Overlay.Scripts.Alerts.RaidAlert;

public partial class Peepo : RigidBody3D
{
	[Export] public Sprite3D[] Sprites;
	[Export] private Node3D _parashute;
	private Sprite3D _selectedSprite;
	private double _timeSinceSpawn;
	private double _timeTillStartMoving = 3d;
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_selectedSprite = Sprites[new Random().Next(0, Sprites.Length)];
		_selectedSprite.Visible = true;
		//add animation here which is a child of the sprite
		_animationPlayer = _parashute.GetNode<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.Play("JumpOut");
		_timeTillStartMoving = _animationPlayer.CurrentAnimationLength;
		
		Freeze = true;		
		LinearVelocity = new Vector3((float)(new Random().NextDouble() - 0.5f), -0.1f, 0);
		
		//disable collisions 
		GetChild<CollisionShape3D>(1).Disabled = true;
	}
	
	public override void _Process(double delta)
	{
		_timeSinceSpawn += delta;
		if (_timeSinceSpawn >= _timeTillStartMoving)
		{
			if (Freeze)
			{
				Freeze = false;
				GetChild<CollisionShape3D>(1).Disabled = false;
				Reparent(GetParent().GetParent().GetParent());
			}
		}
		
		if (GlobalPosition.Y <=0)
		{
			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		//limit Y velocity to 10m/s
		LinearVelocity = new Vector3(LinearVelocity.X, Mathf.Clamp(LinearVelocity.Y,-0.8f,10f), LinearVelocity.Z);
	}
}
