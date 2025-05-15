using Godot;

namespace Temptica.Overlay.Scripts.Alerts.RaidAlert;

public partial class Plane : Node3D
{
	private float _speed = 0.70f;
	private bool _isMoving;
	public int PeepoCount { get; set; }
	
	public override void _Ready()
	{
		var spawner = GetNode<PeepoSpawner>("PeepoSpawner");
		spawner.PeeposToSpawn = PeepoCount;
	}

	public override void _Process(double delta)
	{ 
		Translate(new Vector3((float)(-_speed * delta), 0, 0));
		
		if (GlobalPosition.X + 3 < 0)
		{
			QueueFree();
		}
	}
}
