using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;
using Temptica.Overlay.Scripts.Spawners.Spawnables;
using TwitcherSharp.Chat;

namespace Temptica.Overlay.Scripts.Spawners;

public enum LaunchAbles
{
    Godot,
    Bean,
    Banana,
    Blob,
}

public partial class CannonLauncher : Node3D
{
    [Export] public Node3D Target;
    [Export] public float SpawnRate = 0.25f;
    private double _timeTillNextSpawn;
    private PackedScene _godotThrowable;
    private PackedScene _beanThrowable;
    private PackedScene _bananaThrowable;
    private PackedScene _blobThrowable;

    private readonly Queue<LaunchAbles> _itemsToSpawn = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _godotThrowable = GD.Load<PackedScene>("res://Templates/godot_plushie.tscn");
        _beanThrowable = GD.Load<PackedScene>("res://Templates/bean.tscn");
        _bananaThrowable = GD.Load<PackedScene>("res://Templates/banana.tscn");
        _blobThrowable = GD.Load<PackedScene>("res://Templates/TempticBlob.tscn");

        var throwCommand = TwitchCommand.FromObject(GetNode<GodotObject>("ThrowCommand"));

        throwCommand.CommandReceived += (_, _, args) =>
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;

            if (args.Length > 0 && Enum.TryParse<LaunchAbles>(textInfo.ToTitleCase(args[0]), out var launchAble))
            {
                _itemsToSpawn.Enqueue(launchAble);
                return;
            }

            var items = Enum.GetValues<LaunchAbles>();
            var randomItem = items[new Random().Next(0, items.Length)];
            _itemsToSpawn.Enqueue(randomItem);
        };

        var throwBeanCommand = TwitchCommand.FromObject(GetNode<GodotObject>("ThrowBeanCommand"));
        
        throwBeanCommand.CommandReceived += (_, _, args) =>
        {
            if (args.Length > 0 && int.TryParse(args[0], out var count))
            {
                count = count > 69 ? 69 : count;
                for (var i = 0; i < count; i++)
                {
                    _itemsToSpawn.Enqueue(LaunchAbles.Bean);
                }

                return;
            }

            _itemsToSpawn.Enqueue(LaunchAbles.Bean);
        };
    }

    public override void _Process(double delta)
    {
        if (_itemsToSpawn.Count == 0) return;
        if (_itemsToSpawn.Peek() == LaunchAbles.Bean)
        {
            _itemsToSpawn.Dequeue();
            LaunchBean();
            return;
        }

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
            case LaunchAbles.Banana:
                LaunchBanana();
                break;
            case LaunchAbles.Blob:
                LaunchBlob();
                break;
        }

        _timeTillNextSpawn = 0;
    }

    private void LaunchBlob()
    {
        var blob = _blobThrowable.Instantiate<TempticBlob>();
        LaunchItem(blob);
    }

    private void LaunchBanana()
    {
        var banana = _bananaThrowable.Instantiate<Banana>();
        LaunchItem(banana);
    }

    private void LaunchItem(Spawnable spawnable)
    {
        AddChild(spawnable);
        spawnable.GlobalPosition = new Vector3(GlobalPosition.X, GlobalPosition.Y, 0);

        var velocity = CalculateShotVelocity(spawnable.GlobalPosition);
        spawnable.LinearVelocity = velocity;
    }

    private void LaunchBean()
    {
        var bean = _beanThrowable.Instantiate<Bean>();
        LaunchItem(bean);
        //Send a bean msg to IRadDev
    }

    private void LaunchGodot()
    {
        var plushie = _godotThrowable.Instantiate<Plushie>();
        LaunchItem(plushie);
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