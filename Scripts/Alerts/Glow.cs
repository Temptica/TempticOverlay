using System;
using Godot;
using TwitcherSharp.Extensions;
using TwitcherSharp.Reward;

namespace Temptica.Overlay.Scripts.Alerts;

public partial class Glow : MeshInstance3D
{
    private StandardMaterial3D _material;
    private Color _lastColor;
    private Tween _rainbowTween;
    private static OverlayColorManager OverlayColorManager => OverlayColorManager.Instance;

    public override void _Ready()
    {
        try
        {
            _material = Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
            _lastColor = _material!.AlbedoColor;
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }

        var colorRedeem = GetNodeOrNull<GodotObject>("OverlayColorRedeemListener");
        colorRedeem?.ConnectRedeemed(SetColor);

        _rainbowTween = GetTree()
            .CreateTween()
            .SetLoops();

        _rainbowTween.TweenProperty(_material, "albedo_color", Colors.Orange, 1f).SetTrans(Tween.TransitionType.Cubic);
        _rainbowTween.TweenProperty(_material, "albedo_color", Colors.Yellow, 1f).SetTrans(Tween.TransitionType.Cubic);
        _rainbowTween.TweenProperty(_material, "albedo_color", Colors.Green, 1).SetTrans(Tween.TransitionType.Cubic);
        _rainbowTween.TweenProperty(_material, "albedo_color", Colors.LightBlue, 1)
            .SetTrans(Tween.TransitionType.Cubic);
        _rainbowTween.TweenProperty(_material, "albedo_color", Colors.Red, 1).SetTrans(Tween.TransitionType.Cubic);
        _rainbowTween.Pause();

        OverlayColorManager.OnStartPartyMode += () => StartRainbowMode();
        OverlayColorManager.OnStopPartyMode += StopRainbowMode;
        OverlayColorManager.SetGlowColor += SetColor;
    }

    private void SetColor(TwitchRedemption redemption)
    {
        var msg = redemption.UserInput;
        
        SetColor(new Color(msg));
    }


    private void SetColor(Color color)
    {
        try
        {
            _lastColor = color;
            if (OverlayColorManager.PartyModeEnabled) return;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_material, "albedo_color", new Color(_lastColor), 0.5f)
                .SetTrans(Tween.TransitionType.Cubic);
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
        }
    }

    private void StartRainbowMode(float speedScale = 0.5f)
    {
        _rainbowTween.Play();
        _rainbowTween.SetSpeedScale(speedScale);
    }

    private void StopRainbowMode()
    {
        _rainbowTween.Stop();
        SetColor(_lastColor);
    }
}