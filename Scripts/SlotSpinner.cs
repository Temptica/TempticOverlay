using System;
using Godot;

namespace Temptica.Overlay.Scripts;

public partial class SlotSpinner : Node3D
{
    [Export]
    private AnimatedSprite3D _rainbowFish;
    private double _spinSpeedMax;
    private double _spinSlowDown = 2;
    private double _spinSpeed;
    private SpinnerState _spinnerState = SpinnerState.Stopped;
    private SpinFishType _spinFishType;

    public static readonly SpinFishType[] SpinnersFish = [
        SpinFishType.Blue, SpinFishType.Rainbow, SpinFishType.Red,SpinFishType.Purple, SpinFishType.Blue,  SpinFishType.Orange,
        SpinFishType.Red,SpinFishType.Green
    ];

    public override void _Ready()
    {
        _rainbowFish.Play("gif");
    }
    public void Spin()
    {
        //starts spin 
        _spinnerState = SpinnerState.SpeedingUp;
        _spinSpeedMax = (new Random().NextDouble()+1)*2.0d;
        _spinFishType = (SpinFishType)new Random().Next(0, Enum.GetValues(typeof(SpinFishType)).Length);
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_spinnerState)
        {
            case SpinnerState.SpeedingUp:
                _spinSpeed += _spinSlowDown*5*delta;
                if (_spinSpeed >= _spinSpeedMax)
                {
                    _spinnerState = SpinnerState.SlowingDown;
                }
                break;
            //slow down
            case SpinnerState.SlowingDown when _spinSpeed > 0.0001f :
                _spinSpeed -= _spinSlowDown*delta;
                break;
            //stop
            case SpinnerState.SlowingDown:
                var target = (int)_spinFishType * 45;
                RotationDegrees = new Vector3(target, RotationDegrees.Y, RotationDegrees.Z);
                _spinSpeed = 0;
                _spinnerState = SpinnerState.Stopped;
                break;
            default: return;
        }
        RotateX((float)_spinSpeed);
    }

    public bool IsSpinning()
    {
        return _spinnerState != SpinnerState.Stopped;
    }

    public SpinFishType GetSpinResult() => _spinFishType;
}

public enum SpinFishType
{
    Blue, Rainbow, Red, Purple, Orange, Green
}

public enum SpinnerState
{
    Stopped, SpeedingUp, SlowingDown
}