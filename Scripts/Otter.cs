using System;
using Godot;
using Temptica.GodotExtensions;
using Temptica.Overlay.Scripts.Interfaces;
using Temptica.Overlay.Scripts.SignalR.Listeners;

namespace Temptica.Overlay.Scripts;

public partial class Otter : StaticBody3D, IDraggable
{
	[Export] private MeshInstance3D _thisIsFine = null!;
	[Export] private Sprite3D _texture = null!;
	[Export] private AnimationPlayer _animation = null!;

	public static EventHandler<bool> ShowHideOtterEvent;
	public static EventHandler<bool> ZoomOtterEvent;
	public static EventHandler ExplodeEvent;
	public Vector3 OriginalPosition;
	public Vector3 OriginalSize;
	private Marker3D _nodeBoop;

	private double _squishTimeout;
	
	private double RemainingTime { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_nodeBoop = GetNode<Marker3D>("NodeBoop");
		OriginalPosition = GlobalPosition;
		OriginalSize = Scale;
		OtterSignalRListener.ThisIsFine += (_, _) => { RemainingTime += 10; };

		OtterSignalRListener.Squish += SquishThatOtter;

		ShowHideOtterEvent += (_, show) =>
		{
			if (show)
			{
				CallDeferred("show");
				return;
			}

			CallDeferred("hide");

		};
		ZoomOtterEvent += (_, zoom) =>
		{
			if (zoom)
			{
				SetDeferred("position", new Vector3(8, 4.5f, -0.1f));
				SetDeferred("scale", new Vector3(3, 3, 3));
				return;
			}

			SetDeferred("global_position", OriginalPosition);
			SetDeferred("scale", OriginalSize);
		};

	}

	public override void _Process(double delta)
	{
		if (RemainingTime > 0)
		{
			RemainingTime -= delta;
			if(!_thisIsFine.IsVisible())
			{
				_thisIsFine.Visible = true;
				var tween = _thisIsFine.CreateTween();
				tween.TweenProperty(_thisIsFine, "position", new Vector3(_thisIsFine.Position.X, -0.2f, _thisIsFine.Position.Z),1f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
			}
		}
		else if(_thisIsFine.IsVisible())
		{
			var tween = _thisIsFine.CreateTween();
			tween.TweenProperty(_thisIsFine, "position", new Vector3(_thisIsFine.Position.X, -2, _thisIsFine.Position.Z),1f).SetEase(Tween.EaseType.OutIn).SetTrans(Tween.TransitionType.Quad);
			tween.Finished += () => _thisIsFine.Hide();
		}

		if (_squishTimeout > 0)
		{
			_squishTimeout -= delta;
		}
	}

	public bool IsNoseClick(Vector3 position)
	{
		var nosePosition = _nodeBoop.GetGlobalPosition();
		return nosePosition.DistanceTo(position) < 0.2f;
	}

	private void SquishThatOtter(object sender, EventArgs e)
	{
		if (!(_squishTimeout <= 0)) return;
		_animation.CallAsync(AnimationPlayer.MethodName.Play,"Squish");
		_squishTimeout = 60; //1 minutes
	}
}