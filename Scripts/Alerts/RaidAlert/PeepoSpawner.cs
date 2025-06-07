using Godot;

namespace Temptica.Overlay.Scripts.Alerts.RaidAlert;

public partial class PeepoSpawner : Node3D
{
	
	public const double PeepoSpawnTime = 1d;
	public const double TimeTillFirstPeepo = 3d;
	
	public int PeeposToSpawn { get; set; }
	
	private PackedScene _peepoScene;
	private double _timeSinceLastPeepo;
	private bool _spawnedFirst;

	public override void _Ready()
	{
		_peepoScene = GD.Load<PackedScene>("res://Templates/peepo.tscn");
	}

	public override void _Process(double delta) 
	{
		_timeSinceLastPeepo += delta;
		if(!_spawnedFirst)
		{
			if (_timeSinceLastPeepo <= TimeTillFirstPeepo)
			{
				return;
			}
			_spawnedFirst = true;
		}
		if (_timeSinceLastPeepo >= PeepoSpawnTime && PeeposToSpawn > 0)
		{
			_timeSinceLastPeepo = 0;
			var instance = (Peepo)_peepoScene.Instantiate();
			AddChild(instance);
			PeeposToSpawn--;
		}
	}
	
}