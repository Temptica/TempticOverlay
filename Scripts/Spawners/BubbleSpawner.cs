using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptica.Overlay.Templates;

namespace Temptica.Overlay.Scripts.Spawners;

public partial class BubbleSpawner : Node3D
{
    private PackedScene _bubbleScene;

    public static EventHandler<Vector3?> SpawnBubble;
    private List<Bubble> _bubbles = [];
    
    public static BubbleSpawner Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        _bubbleScene = GD.Load<PackedScene>("res://Templates/bubble.tscn");
        SpawnBubble += (obj , pos) =>
        {
            pos ??= GlobalPosition;
            var bubble = (Bubble)_bubbleScene.Instantiate();
            
            CallDeferred("add_child", bubble);
            
            bubble.CallDeferred("set_global_position", pos.Value);
            
            bubble.InitialX = pos.Value.X;
            if (obj is scenes.BubbleMachineSpawner)
            {
                bubble.CallDeferred("set_linear_velocity", new Vector3(0,2,0));
                
            }
        };
    }

    public override void _Process(double delta)
    {
        _bubbles = GetChildren().Cast<Bubble>().ToList();
    }

    public bool CheckBubbleHit(Vector3 clickPos)
    {
        try
        {
            var children = _bubbles.ToList();
                
            var result = children
                .Where(b => b != null)
                .Count(child => child.CheckClick(clickPos));
            Labels.ClickCounterDisplay.UpdateBubbles(result);
            return result>0;
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        return false;
    }
}