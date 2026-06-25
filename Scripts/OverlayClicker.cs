using System;
using System.Threading.Tasks;
using Godot;
using Temptica.GreenHeatSharp;
using Temptica.Overlay.Enums;
using Temptica.Overlay.Scripts.Easter;
using Temptica.Overlay.Scripts.Fishes;
using Temptica.Overlay.Scripts.Labels;
using Temptica.Overlay.Scripts.Models;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.Spawners;
using BubbleSpawner = Temptica.Overlay.Scripts.Spawners.BubbleSpawner;
using SnowSpawner = Temptica.Overlay.Scenes.SnowSpawner;

namespace Temptica.Overlay.Scripts;

public partial class OverlayClicker : Node3D
{
	private PackedScene _clickScene;
	[Export] private EggSpawner _eggSpawner;
	[Export] private Otter _otter;

	public override void _Ready()
	{
		_clickScene = GD.Load<PackedScene>("res://Templates/click.tscn");
	}

	public override async void _Input(InputEvent @event)
	{
		var message = (@event as InputEventMouseButton)?.AsGreenHeatMessage();
		if (message is not { Type: GreenHeatMessageType.Click }) return;
		var userId = message.Id;
		
		var user = message.IsAnonymous!.Value
			? new User { Id =userId, Username = "Anonymous" }
			: await UserService.Instance.GetOrCreateUser(userId);

		var clickModel = new OverlayClickModel(message.X, message.Y,
			user.Username, userId, user.Color);
		_ = OnClick(clickModel);
	}

	private async Task OnClick(OverlayClickModel clickModel)
	{
		var click = (Click)_clickScene.Instantiate();
		click.OverlayClickModel = clickModel;

		AddChild(click);
		
		if(clickModel.Anonymous) return;

		var x = clickModel.X;
		var y = clickModel.Y;

		var clickPos = new Vector3(x * 16f, 9f - y * 9f, 0);
		var clickPos2D = new Vector2(clickPos.X, clickPos.Y);

		SnowSpawner.CheckPackages(clickPos2D, clickModel.Username);

		//Nose boops
		if (_otter.IsNoseClick(clickPos))
		{
			ClickCounterDisplay.UpdateNose();
			if (new Random().Next(0, 100) < 3)
			{
				AudioPlayer.PlayAudio(AudioEffects.Otter3);
			}
		}

		//check hits any of teh fishes
		if (FishSpawner.CheckFishesHit(clickPos2D, clickModel.Username, out var points))
		{
			
		}

		if (RandomTrashSpawner.CheckTrashHit(clickPos2D, clickModel.Username,
				out var pointsTrash))
		{
			
		}

		if (await _eggSpawner.IsEggHit(clickPos2D, clickModel))
		{
			return;
		}

		if (BubbleSpawner.Instance.CheckBubbleHit(clickPos))
			return;

		if (new Random().Next(0, 10) < 3)
		{
			BubbleSpawner.Instance.SpawnBubble(clickPos);
		}
	}
}
