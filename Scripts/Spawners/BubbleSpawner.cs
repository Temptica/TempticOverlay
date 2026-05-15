using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptica.Overlay.Scripts.Labels;
using Bubble = Temptica.Overlay.Scripts.Spawners.Spawnables.Bubble;

namespace Temptica.Overlay.Scripts.Spawners;

public partial class BubbleSpawner : Node3D
{
    private PackedScene _bubbleScene;
    private List<Bubble> _bubbles = [];
    private float _timer;
    [Export] private double _spawnTime = 0.01f;
    private double _remainingTimeTillNext;

    public static BubbleSpawner Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _bubbleScene = GD.Load<PackedScene>("res://Templates/bubble.tscn");
        _remainingTimeTillNext = _spawnTime;
    }

    public void AddBubbleTime(float bubbleTime)
    {
        GD.Print($"Added {bubbleTime}s");
        _timer += bubbleTime;
    }

    public void SpawnBubble(Vector3? pos = null)
    {
        pos ??= GlobalPosition;
        var bubble = (Bubble)_bubbleScene.Instantiate();
        
        AddChild(bubble);
        bubble.SetGlobalPosition(pos.Value);
        bubble.SetRotation(Vector3.Zero);

        bubble.InitialX = pos.Value.X;
        
        bubble.SetLinearVelocity(new Vector3(0, 2, 0));
    }

    public override void _Process(double delta)
    {
        _bubbles = GetChildren().Cast<Bubble>().ToList();

        if (!(_timer > 0)) return;

        _remainingTimeTillNext -= delta;

        if (_remainingTimeTillNext <= 0)
        {
            SpawnBubble();
            _remainingTimeTillNext = _spawnTime;
        }

        _timer -= (float)delta;
    }

    public bool CheckBubbleHit(Vector3 clickPos)
    {
        try
        {
            var children = _bubbles.ToList();

            var result = children
                .Where(b => b != null)
                .Count(child => child.CheckClick(clickPos));
            ClickCounterDisplay.UpdateBubbles(result);
            return result > 0;
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        return false;
    }
}