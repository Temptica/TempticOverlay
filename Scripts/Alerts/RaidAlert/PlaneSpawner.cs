using System;
using System.Linq;
using Godot;

namespace Temptica.Overlay.Scripts.Alerts.RaidAlert;

public partial class PlaneSpawner : Node3D
{
	public const int RaidDuration = 33;
	const double NormalPlaneSpawnRate = 4d;
	private const int PeepoPerPlane = 20;
	
	private PackedScene _planeScene;
	private Plinko _plinko;
	
	private int _planesToSpawn;
	private int _peeposDropped;
	private int _peeposToDrop;
	private double _timeSinceLastPlane;
	private double _planeSpawnRate = NormalPlaneSpawnRate;
	public override void _Ready()
	{
		_planeScene = GD.Load<PackedScene>("res://Templates/plane.tscn");
		RaidAlert.StartPlanes += OnStartPlanes;
		_plinko = GetNode<Plinko>("%Plinko");
		_timeSinceLastPlane = NormalPlaneSpawnRate;
	}

	public override void _Process(double delta)
	{
		if(!_plinko.Visible) return;
		if (Remaining == 0 && _planesToSpawn == 0)
		{
			var peepos = GetChildren().OfType<Plane>().Sum(p => p.GetChild<PeepoSpawner>(1).GetChildren().OfType<Peepo>().Count()) + GetChildren().OfType<Peepo>().Count();
				
			if (_timeSinceLastPlane >= _planeSpawnRate && peepos == 0 && Remaining == 0)
			{
				_plinko.HidePlinko();
				return;
			}
		}
		
		_timeSinceLastPlane += delta;
		if (_timeSinceLastPlane >= _planeSpawnRate && _planesToSpawn > 0)
		{
			_timeSinceLastPlane = 0;
			SpawnPlane();
		}
	}
	
	private int Remaining => _peeposToDrop - _peeposDropped;
	private int NextDropAmount => Remaining >= PeepoPerPlane ? PeepoPerPlane : Remaining;

	private void OnStartPlanes(object sender, int amountToDrop)
	{
		var planesCount = (int)Math.Ceiling(amountToDrop / (double)PeepoPerPlane);
		
		var expectedTime = planesCount * NormalPlaneSpawnRate + PeepoSpawner.TimeTillFirstPeepo + PeepoSpawner.PeepoSpawnTime * PeepoPerPlane;
		
		
		if(expectedTime > RaidDuration + 10)
			_planeSpawnRate = RaidDuration / (double)planesCount;
		else
			_planeSpawnRate = NormalPlaneSpawnRate;
		
		_peeposToDrop += amountToDrop;
		_planesToSpawn += planesCount;
		_plinko.ShowPlinko();
		
		GD.Print($"Expected time: {expectedTime}. Planes: {planesCount}, Peepos: {amountToDrop}, Plane spawn rate: {_planeSpawnRate}");
		
		if (_peeposDropped != amountToDrop) return;
		SpawnPlane();
		
	}

	private int spawnCounter;
	const float baseAdjustment = 1f; 
	private void SpawnPlane()
	{
		var instance = (Plane)_planeScene.Instantiate();
		instance.PeepoCount = NextDropAmount;
		AddChild(instance);
		_peeposDropped += NextDropAmount;
		_planesToSpawn--;
		
		//if spawn rate is faster than normal, but not twice as fast, spawn one plane lower. If it is twice or more faster, spawn one plane higher, then one even lower if it's 3 times faster, etc.
		var spawnRateRatio = NormalPlaneSpawnRate / _planeSpawnRate;
		

		// Apply the dynamic adjustment
		if (spawnRateRatio > 1)
		{
			GD.Print(spawnCounter);
			switch (spawnCounter)
			{
				case 1:
					instance.Translate(new Vector3(0, baseAdjustment, 0));
					break;
				case 2:
					instance.Translate(new Vector3(0, -baseAdjustment, 0));
					break;
				default:
					spawnCounter = 0;
					break;
			}
			spawnCounter++;
		}
		
	}
}
