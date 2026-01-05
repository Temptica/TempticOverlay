using Godot;
using Godot.Collections;

namespace Temptica.Overlay.Scripts.Labels;

public partial class PartyModeLabel : Label3D
{
	private Tween _rainbowTween;
	private bool _RainbowMode;

	public override void _Ready()
	{
		_rainbowTween = GetTree()
			.CreateTween()
			.SetLoops();

		_rainbowTween.TweenProperty(this, "modulate", Colors.Orange, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Yellow, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Green, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.LightBlue, 1)
			.SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Red, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.Pause();
	}
	
	public void StartPartyMode()
	{
		if(_RainbowMode) return;
		_rainbowTween.Play();
		_rainbowTween.SetSpeedScale(0);
		_RainbowMode = true;
		CallDeferred(Node3D.MethodName.Show);
	}
	
	public void EndPartyMode()
	{
		if(!_RainbowMode) return;
		_RainbowMode = false;
		_rainbowTween.Stop();
		CallDeferred(Node3D.MethodName.Hide);
	}
}
