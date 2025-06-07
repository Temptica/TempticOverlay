using System;
using System.Globalization;
using Godot;
using Temptica.Overlay.Scripts.Easter;
using Temptica.Overlay.Scripts.Fishes;
using Temptica.Overlay.Scripts.Labels;
using Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptica.Overlay.Scripts.Spawners;
using Temptica.TwitchBot.Shared.enums;
using BubbleSpawner = Temptica.Overlay.Scripts.Spawners.BubbleSpawner;

namespace Temptica.Overlay.Scripts;

public partial class OverlayClicker : Node3D
{
    private PackedScene _clickScene;
    private BubbleSpawner _bubbleSpawner;
    [Export] private EggSpawner _eggSpawner;
    [Export] private Otter _otter;

    public override void _Ready()
    {
        _clickScene = GD.Load<PackedScene>("res://Templates/click.tscn");
        _bubbleSpawner = GetNode<BubbleSpawner>("%BubbleSpawner");

        OverlayClickListener.OverlayClick += async (_, model) =>
        {
            var click = (Click)_clickScene.Instantiate();
            click.OverlayClickModel = model;

            CallDeferred(Node.MethodName.AddChild, click);

            var x = float.Parse(model.X, CultureInfo.InvariantCulture);
            var y = float.Parse(model.Y, CultureInfo.InvariantCulture);

            var clickPos = new Vector3(x * 16f, 9f - y * 9f, 0);
            var clickPos2D = new Vector2(clickPos.X, clickPos.Y);

            //Nose boops
            if (await _otter.IsNoseClick(clickPos))
            {
                ClickCounterDisplay.UpdateNose();
                if (new Random().Next(0, 100) < 10)
                {
                    AudioPlayer.PlayAudio(AudioEffects.Otter3);
                }
            }

            //check hits any of teh fishes
            if (FishSpawner.CheckFishesHit(clickPos2D, model.Username, out var points))
            {
                Overlay.SignalRService.FishClicked(model.Username, points);
            }

            if (RandomTrashSpawner.CheckTrashHit(clickPos2D, model.Username,
                    out var pointsTrash))
            {
                Overlay.SignalRService.TrashClicked(model.Username, pointsTrash);
            }

            if (_eggSpawner.IsEggHit(clickPos2D, out var eggType))
            {
                Overlay.SignalRService.EggClicked(model.Username, (int)eggType!.Value);
                return;
            }
            
            Overlay.SignalRService.Clicked(model.Username);

            if (_bubbleSpawner.CheckBubbleHit(clickPos))
                return;

            if (new Random().Next(0, 10) < 3)
            {
                BubbleSpawner.SpawnBubble?.Invoke(this, clickPos);
            }
        };
    }
}