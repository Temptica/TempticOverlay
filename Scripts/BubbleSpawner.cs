using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Temptic404Overlay.Templates;

namespace Temptic404Overlay.Scripts;

public partial class BubbleSpawner : Node3D
{
    private PackedScene _bubbleScene;

    public static EventHandler<Vector3> SpawnBubble;
    private List<Bubble> _bubbles = [];
    public override void _Ready()
    {
        _bubbleScene = GD.Load<PackedScene>("res://Templates/bubble.tscn");
        SpawnBubble += (_, pos) =>
        {
            var bubble = (Bubble)_bubbleScene.Instantiate();
            
            CallDeferred("add_child", bubble);
            
            bubble.CallDeferred("set_global_position", pos);
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

            return children
                .Where(b => b != null)
                .Any(child => child.CheckClick(clickPos));
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        return false;
    }
}