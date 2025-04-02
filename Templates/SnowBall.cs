using System.Linq;
using Godot;

namespace Temptic404Overlay.Templates;

public partial class SnowBall : RigidBody3D
{
	[Export] private Sprite3D _snowBallSprite;
	[Export] private Sprite3D _snowSplashSprite;
	private bool isHiding = false;
	private Tween _tween;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//check collision
		var col = GetCollidingBodies().FirstOrDefault();
		if ((isHiding && _tween != null && !_tween.IsRunning())||GlobalPosition.Y < -1)
		{
			QueueFree();
			return;
		}
		
		if (col == null || isHiding) return;
		
		_snowBallSprite.Hide();
		_snowSplashSprite.Show();
		SetSleeping(true);
		
		isHiding = true;

		_tween = CreateTween();
		//fade out by 
		_tween.Parallel().TweenProperty(_snowSplashSprite, "global_position",
			new Vector3(GlobalPosition.X, GlobalPosition.Y - 0.5f, GlobalPosition.Z), 2.9f)
			.SetEase(Tween.EaseType.In)
			.SetDelay(0.1f);
		_tween
			.Parallel()
			.TweenProperty(_snowSplashSprite,"modulate", new Color(255, 255, 255, 0),2)
			.SetEase(Tween.EaseType.In)
			.SetDelay(1);
	}
}