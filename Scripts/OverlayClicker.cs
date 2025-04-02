using Godot;
using System;
using System.Globalization;
using Temptic404Overlay.Scripts;
using Temptic404Overlay.Scripts.Fishes;
using Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptica.TwitchBot.Shared.enums;

public partial class OverlayClicker : Node3D
{
	private PackedScene _clickScene;
	private BubbleSpawner _bubbleSpawner;
	[Export] private Otter _otter;
	public override void _Ready()
	{
		_clickScene = GD.Load<PackedScene>("res://Templates/click.tscn");
		_bubbleSpawner = GetNode<BubbleSpawner>("%BubbleSpawner");
		
		OverlayClickListener.OverlayClick += (_, model) =>
		{
			var click = (Click)_clickScene.Instantiate();
			click.OverlayClickModel = model;
			
			var x = float.Parse(model.X, CultureInfo.InvariantCulture);
			var y = float.Parse(model.Y, CultureInfo.InvariantCulture);
			var clickPos = new Vector3(x*16f,  9f - y*9f,0);

			
			GD.Print($"Click overlay received: ({x}, {y})");
			//Nose boops
			if(_otter.IsNoseClick(clickPos))
			{
				ClickCounterDisplay.UpdateNose();
				if (new Random().Next(0, 100) < 10)
				{
					AudioPlayer.PlayAudio(AudioEffects.Otter3);
				}
			}
			
			CallDeferred(Node.MethodName.AddChild, click);
			GD.Print($"Click X: {clickPos.X} Y: {clickPos.Y}");
			
			//check hits any of teh fishes
			if (FishSpawner.CheckFishesHit(new Vector2(clickPos.X, clickPos.Y), model.Username, out var points))
			{
				click.AddPoints(points);
			}

			if (RandomTrashSpawner.CheckTrashHit(new Vector2(clickPos.X, clickPos.Y), model.Username, out var pointsTrash))
			{
				click.AddPoints(pointsTrash);
			}
			
			if(_bubbleSpawner.CheckBubbleHit(clickPos))
				return;
			
			if (new Random().Next(0, 10) < 3)
			{
				BubbleSpawner.SpawnBubble?.Invoke(this, clickPos);
			}
		}; 
	}
}
