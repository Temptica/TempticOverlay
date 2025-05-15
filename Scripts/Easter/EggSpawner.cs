using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.Easter;

public partial class EggSpawner : Node3D
{
    [Export] private int _minSpawnInterval = 5;
    [Export] private int _maxSpawnInterval = 10;
    private DateTime _nextSpawnTime = DateTime.MinValue;
    private bool _enabled;

    private List<Egg> _eggs;

    PackedScene _eggScene;

    public override void _Ready()
    {
        _eggScene = GD.Load<PackedScene>("res://scenes/easter/egg.tscn");
        _enabled = DateTime.Now.Month == 4; //Only during the month of Easter so April
    }

    public override void _Process(double delta)
    {
        if (!_enabled) return;
        _eggs = GetChildren().OfType<Egg>().ToList();

        if (_nextSpawnTime >= DateTime.Now) return;

        //spawn logic
        var egg = _eggScene.Instantiate() as Egg;
        egg!.SetSprite((EggType)new Random().Next(Enum.GetValues(typeof(EggType)).Length));
        AddChild(egg);
        egg.GlobalPosition = new Vector3((float)new Random().NextDouble() * 14 + 1,
            (float)new Random().NextDouble() * 7 + 1, 0);
        _nextSpawnTime = DateTime.Now.AddMinutes(new Random().Next(_minSpawnInterval, _maxSpawnInterval));
    }

    public bool IsEggHit(Vector2 position, out EggType? eggType)
    {
        if (!_enabled)
        {
            eggType = null;
            return false;
        }

        var egg = _eggs.FirstOrDefault(e => e.IsHit(position));

        egg?.QueueFree();

        eggType = egg?.EggType;
        return egg != null;
    }
}