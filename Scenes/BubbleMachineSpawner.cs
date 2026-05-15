using Godot;
using TwitcherSharp.Reward;
using BubbleSpawner = Temptica.Overlay.Scripts.Spawners.BubbleSpawner;

namespace Temptica.Overlay.Scenes;

public partial class BubbleMachineSpawner : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("ready machine");
        var node = GetNode<GodotObject>("SpawnBubbelsRedeemListener");
        var twitchRedeemListener = TwitchRedeemListener.FromObject(node);
        twitchRedeemListener.Redeemed += _ =>
        {
            GD.Print("fullfilling");
            BubbleSpawner.Instance.AddBubbleTime(1);
        };
    }
}