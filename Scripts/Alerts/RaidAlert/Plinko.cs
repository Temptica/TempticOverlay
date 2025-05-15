using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Temptica.Overlay.Scripts.Alerts.RaidAlert;

public partial class Plinko : Node3D
{
	private AnimationPlayer _animationPlayer;
	public new bool Visible { get; private set; }

	private List<CollisionShape3D> _pins;
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Hide();
		_animationPlayer.AnimationFinished += AnimationPlayerOnAnimationFinished;

		_pins = GetChild<Node3D>(1)
			.GetChildren()
			.OfType<MeshInstance3D>()
			.Select(p=>p.GetChild(0).GetChild(0) as CollisionShape3D)
			.ToList();
		_pins.ForEach(pin => pin.Disabled = true);
	}

	private void AnimationPlayerOnAnimationFinished(StringName name)
	{
		if (name != "ShowPlinko") return;
		if (Visible)
		{
			_animationPlayer.Play("MoveWall");			
			return;
		}
		//Hide();
	}
	
	public void HidePlinko()
	{
		if(!Visible) return;
		Visible = false;
		_animationPlayer.Stop();
		_animationPlayer.PlayBackwards("ShowPlinko");
		_pins.ForEach(pin => pin.Disabled = true);
		
	}
	
	public void ShowPlinko()
	{
		if (Visible) return;
		Visible = true;
		_animationPlayer.Stop();
		_animationPlayer.Play("ShowPlinko");
		_pins.ForEach(pin => pin.Disabled = false);
		//
		Show();
		
	}
}
