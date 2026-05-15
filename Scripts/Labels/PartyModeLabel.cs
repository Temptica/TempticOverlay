using Godot;

namespace Temptica.Overlay.Scripts.Labels;

public partial class PartyModeLabel : Label3D
{
	private Tween _rainbowTween;

	public override void _Ready()
	{
		_rainbowTween = GetTree()
			.CreateTween()
			.SetLoops();

		_rainbowTween.TweenProperty(this, "modulate", Colors.Orange, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Yellow, 1f).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Green, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.LightBlue, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.TweenProperty(this, "modulate", Colors.Red, 1).SetTrans(Tween.TransitionType.Cubic);
		_rainbowTween.Pause();

		OverlayColorManager.Instance.OnStartPartyMode += StartPartyMode;
		OverlayColorManager.Instance.OnStopPartyMode += StopPartyMode;
	}

	private void StartPartyMode()
	{
		_rainbowTween.Play();
		_rainbowTween.SetSpeedScale(0.5f);
		Show();
	}

	private void StopPartyMode()
	{
		_rainbowTween.Stop();
		Hide();
	}
}
