using System;
using System.Collections.Generic;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptica.Overlay.Scripts.Spawners.Spawnables;

namespace Temptica.Overlay.Scripts.Spawners;

public enum LaunchAbles
{
    Godot,
    Bean
}

public partial class CannonLauncher : Node3D
{
    [Export] public Node3D Target;
    [Export] public float SpawnRate = 0.75f;
    private double _timeTillNextSpawn;
    private PackedScene _godotThrowable;
    private PackedScene _beanThrowable;

    private readonly Queue<LaunchAbles> _itemsToSpawn = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _godotThrowable = GD.Load<PackedScene>("res://Templates/godot_plushie.tscn");
        _beanThrowable = GD.Load<PackedScene>("res://Templates/bean.tscn");

        ThrowPlushieListener.OnThrowPlushie += (_, _) => _itemsToSpawn.Enqueue(LaunchAbles.Godot);
        SpawnersListener.OnSpawnBean += (_, _) => _itemsToSpawn.Enqueue(LaunchAbles.Bean);
    }

    public override void _Process(double delta)
    {
        if (_timeTillNextSpawn < SpawnRate)
        {
            _timeTillNextSpawn += delta;
            return;
        }

        if (_itemsToSpawn.Count == 0) return;

        switch (_itemsToSpawn.Dequeue())
        {
            case LaunchAbles.Godot:
                LaunchGodot();
                break;
            case LaunchAbles.Bean:
                LaunchBean();
                break;
        }

        _timeTillNextSpawn = 0;
    }

    private void LaunchBean()
    {
        var bean = _beanThrowable.Instantiate<Bean>();
        AddChild(bean);
        bean.GlobalPosition = new Vector3(GlobalPosition.X, GlobalPosition.Y, 0);

        var velocity = CalculateShotVelocity(bean.GlobalPosition);
        bean.LinearVelocity = velocity;
    }

    private void LaunchGodot()
    {
        var plushie = _godotThrowable.Instantiate<Plushie>();
        AddChild(plushie);
        plushie.GlobalPosition = new Vector3(GlobalPosition.X, GlobalPosition.Y, 0);

        var velocity = CalculateShotVelocity(plushie.GlobalPosition);
        plushie.LinearVelocity = velocity;
    }

    private Vector3 CalculateShotVelocity(Vector3 from)
    {
        var additionalHeight = new Random().Next(75, 300) / 100f;

        var to = new Vector3(Target.GlobalPosition.X,
            (float)new Random().NextDouble() * 2f + Target.GlobalPosition.Y, Target.GlobalPosition.Z);

        var maxHeight = Mathf.Max(from.Y, to.Y) + additionalHeight;
        var verticalVelocity = Mathf.Sqrt((maxHeight - from.Y) * 2 * 9.8f);

        var peakTime = verticalVelocity / 9.8f;
        var fallTime = Mathf.Sqrt((maxHeight - to.Y) * 2 / 9.8f);
        var totalTime = peakTime + fallTime;

        var positionDelta = to - from;
        positionDelta.Y = 0;

        var distance = positionDelta.Length();

        var finalVelocity = positionDelta.Normalized() * (distance / totalTime);
        finalVelocity.Y = verticalVelocity;

        return finalVelocity;
    }
}