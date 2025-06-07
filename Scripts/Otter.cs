using System;
using System.Threading.Tasks;
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
	public Vector3 OriginalPosition;

	private double _squishTimeout;
	
	private double RemainingTime { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OriginalPosition = Position;
		GenericSignalRListener.ThisIsFine += (_, _) =>
		{
			RemainingTime += 10;
		};

		GenericSignalRListener.Squish += SquishThatOtter;
		
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
				SetDeferred("position",new Vector3(8, 4.5f, -0.1f));
				SetDeferred("scale", new Vector3(3,3,3));
				return;
			}
			SetDeferred("position",OriginalPosition);
			SetDeferred("scale", new Vector3(1,1,1));
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

	public async Task<bool> IsNoseClick(Vector3 position)
	{
		//if x and Y are in the center of the otter sprite, margin of 0.5, then return true
		var textureGlobalPos = (await _texture.CallAsync(Node3D.MethodName.GetGlobalPosition)).AsVector3();
		var xCenter = textureGlobalPos.X + (await _texture.Texture.CallAsync(Texture2D.MethodName.GetWidth)).AsSingle() / 2f;
		var yCenter = textureGlobalPos.Y + (await _texture.Texture.CallAsync(Texture2D.MethodName.GetHeight)).AsSingle() / 2f;
		var z = position.Z;
		var pos =  new Vector3(xCenter, yCenter, z);
		
		var distance = pos.DistanceTo(position);
		return distance < 1.5f;
	}

	private void SquishThatOtter(object sender, EventArgs e)
	{
		if (!(_squishTimeout <= 0)) return;
		_animation.CallAsync(AnimationPlayer.MethodName.Play,"Squish");
		_squishTimeout = 120; //2 minutes
	}
}