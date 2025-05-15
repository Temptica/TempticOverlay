using System;
using Godot;

namespace Temptica.Overlay.Scripts.Spawners;

public partial class Trash : Node3D
{
	[Export] private Sprite3D[] _trashTextures = null!;

	private Sprite3D _selectedTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_selectedTexture = _trashTextures[new Random().Next(_trashTextures.Length)];
		_selectedTexture.Visible = true;
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GlobalPosition.Y < -3)
		{
			QueueFree();
		}
	}

	public bool IsHit(Vector2 position)
	{
		var pos = new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Y);
		var isHit = position.DistanceTo(pos) <= 0.6f;
		return isHit;
	}
}