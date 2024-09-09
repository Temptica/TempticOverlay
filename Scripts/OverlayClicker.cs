using Godot;
using System;
using Temptic404Overlay.Scripts;
using Temptic404Overlay.Scripts.Fishes;
using Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public partial class OverlayClicker : Node3D
{
	private PackedScene _clickScene;
	private BubbleSpawner _bubbleSpawner;
	public override void _Ready()
	{
		_clickScene = GD.Load<PackedScene>("res://Templates/click.tscn");
		_bubbleSpawner = GetNode<BubbleSpawner>("%BubbleSpawner");
		
		OverlayClickListener.OverlayClick += (_, model) =>
		{
			var click = (Temptic404Overlay.Scripts.Click)_clickScene.Instantiate();
			click.OverlayClickModel = model;
			//addChild in the main thread to avoid threading issues
			CallDeferred(Node.MethodName.AddChild, click);
			var clickPos = new Vector3(float.Parse(model.X)*16,  9 - float.Parse(model.Y)*9,0);
			
			
			//check hits any of teh fishes
			if(FishSpawner.CheckFishesHit(new Vector2(float.Parse(model.X)*16,  9 - float.Parse(model.Y)*9), model.Username))
				return;
			
			if(_bubbleSpawner.CheckBubbleHit(clickPos))
				return;
			
			if (new Random().Next(0, 10) < 3)
			{
				GD.Print("Spawn bubble!");
				BubbleSpawner.SpawnBubble?.Invoke(this, clickPos);
			}
			
		}; 
	}
}
