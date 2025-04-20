using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Swan;
using Temptica.TwitchBot.Shared.enums;

public partial class EggSpawner : Node3D
{
    [Export] private int _minSpawnInterval = 5;
    [Export] private int _maxSpawnInterval = 10;
    private DateTime _nextSpawnTime = DateTime.MinValue;

    private List<Egg> _eggs;

    PackedScene _eggScene;

    public override void _Ready()
    {
        _eggScene = GD.Load<PackedScene>("res://scenes/easter/egg.tscn");
    }

    public override void _Process(double delta)
    {
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
        var egg = _eggs.FirstOrDefault(e => e.IsHit(position));

        egg?.QueueFree();

        eggType = egg?.EggType;
        return egg != null;
    }
}