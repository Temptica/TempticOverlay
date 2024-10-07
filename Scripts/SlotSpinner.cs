using Godot;
using System;

public partial class SlotSpinner : Node3D
{
    [Export]
    private AnimatedSprite3D _rainbowFish;
    private double _spinSpeedMax;
    private double _spinSlowDown = 1;
    private double _spinSpeed;
    private SpinnerState _spinnerState = SpinnerState.Stopped;
    private SpinFishType _spinFishType;
    
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
            case SpinnerState.SlowingDown when _spinSpeed > 0.02f :
                _spinSpeed -= _spinSlowDown*delta;
                break;
            case SpinnerState.SlowingDown when _spinSpeed > 0:
                //smoothly stop spinning to end up on X being _fishType * 45
                var target = (int)_spinFishType * 45;
                if(target > 180)
                    target -= 360;
                if (GlobalRotationDegrees.X < target)
                {
                    GD.Print(Mathf.Lerp(GlobalRotationDegrees.X, target, (float)delta *0.025f));
                    RotateX(Mathf.Lerp(GlobalRotationDegrees.X, target, (float)delta *0.5f));
                    return;
                }
                if (GlobalRotationDegrees.X > target)
                {
                    RotateX(Mathf.Lerp(GlobalRotationDegrees.X, target, (float)delta *0.025f));
                    return;
                }
                _spinnerState = SpinnerState.Stopped;
                GD.Print("Stopped");

                break;
            case SpinnerState.SlowingDown:
                _spinnerState = SpinnerState.Stopped;
                break;
            default: return;
        }
        //rotate the spinner
        RotateX((float)_spinSpeed);
        GD.Print(_spinSpeed);
    }

    public bool IsSpinning()
    {
        return _spinnerState != SpinnerState.Stopped;
    }
}

public enum SpinFishType
{
    Blue, Rainbow, Red, Purple, Pink, Orange, Green, Gray
}

public enum SpinnerState
{
    Stopped, SpeedingUp, SlowingDown
}
