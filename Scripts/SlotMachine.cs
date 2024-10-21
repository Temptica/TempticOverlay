#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.SignalR;

namespace Temptic404Overlay.Scripts;

public partial class SlotMachine : Node3D
{
    [Export]
    private SlotSpinner _leftSpinner = null!;
    [Export]
    private SlotSpinner _centerSpinner = null!;
    [Export]
    private SlotSpinner _rightSpinner = null!;
    [Export]
    private AnimationPlayer _animationPlayer = null!;
    [Export]
    private Label3D _usernameLabel = null!;
    
    private SpinRequest? _currentSpin;
    private bool _allowNewSpin = true;

    private bool _isOnScreen;
    private bool _canHide;
    
    private static readonly List<SpinRequest> SpinQueue = [];

    private static readonly List<SpinValuePair> FishPointsTable = 
    [
        new SpinValuePair(1.2d, [SpinFishType.Blue, SpinFishType.Purple, SpinFishType.Red] ),
        new SpinValuePair(1.2d, [SpinFishType.Red, SpinFishType.Purple, SpinFishType.Blue]),
        new SpinValuePair(1.2d, [SpinFishType.Green, SpinFishType.Blue, SpinFishType.Orange]),
        new SpinValuePair(1.2d, [SpinFishType.Orange, SpinFishType.Blue, SpinFishType.Green]),
        new SpinValuePair(1.2d, [SpinFishType.Orange, SpinFishType.Red, SpinFishType.Purple]),
        new SpinValuePair(1.2d, [SpinFishType.Purple, SpinFishType.Red, SpinFishType.Orange]),
        new SpinValuePair(1.5d, [SpinFishType.Blue,SpinFishType.Orange,SpinFishType.Rainbow]),
        new SpinValuePair(1.5d, [SpinFishType.Rainbow,SpinFishType.Orange,SpinFishType.Blue]),
        new SpinValuePair(3d, [SpinFishType.Rainbow,SpinFishType.Rainbow,SpinFishType.Rainbow]),
        new SpinValuePair(2d, [SpinFishType.Orange,SpinFishType.Orange,SpinFishType.Orange])
    ];
    
    public static void Spin(string username, int betAmount)
    {
        SpinQueue.Add(new SpinRequest(username, betAmount));
    }
    
    public override void _Process(double delta)
    {
        //nothing waiting
        if(_currentSpin is null && SpinQueue.Count == 0 && _canHide)
        {
            if(_isOnScreen && !_animationPlayer.IsPlaying())
            {
                _animationPlayer.Play("Hide");
            }
            return;
        }
        
        //is we have a spin
        if (_currentSpin is not null)
        {
            if(_leftSpinner.IsSpinning() || _centerSpinner.IsSpinning() || _rightSpinner.IsSpinning())
            {
                //still spinning
                return;
            }
            //get the results
            
            var spinResults = new[]
            {
                _leftSpinner.GetSpinResult(),
                _centerSpinner.GetSpinResult(),
                _rightSpinner.GetSpinResult()
            };
            
            var points = CalculatePoints(_currentSpin.PointsUsed, spinResults);
            GD.Print($"User {_currentSpin.Username} won {points} points.");
            Overlay.SignalRService.AddPoints(_currentSpin.Username,points);
            
            _currentSpin = null;

            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                _allowNewSpin = true;
                _canHide = true;
            });
        }
        
        if (SpinQueue.Count <= 0 || _currentSpin is not null || !_allowNewSpin) return;
        _canHide = false;
        //start new spin
        _allowNewSpin = false;
        if (!_isOnScreen && _animationPlayer.IsPlaying())
        {
            return;
        }
        if (!_isOnScreen && !_animationPlayer.IsPlaying())
        {
            
            _animationPlayer.Play("ShowUp");
            return;
        }
        
        var spin = SpinQueue.First();
        SpinQueue.Remove(spin);
        _usernameLabel.Text = spin.Username;
        _currentSpin = spin;
        _animationPlayer.Play("StartSpin");
        _leftSpinner.Spin();
        _centerSpinner.Spin();
        _rightSpinner.Spin();
        _allowNewSpin = false;
    }
    
    private int CalculatePoints(int usedPoints, SpinFishType[] spinResults)
    {
        foreach (var (modifier, value) in FishPointsTable)
        {
            if (value.SequenceEqual(spinResults))
            {
                return (int)Math.Round(modifier*usedPoints);
            }
        }

        if (spinResults[0] != spinResults[1] && spinResults[1] != spinResults[2])
        {
            if (spinResults[0] == spinResults[2])
            {
                return (int)Math.Round(1.1f*usedPoints);
            }

            return 0;
        }

        if (spinResults[1] == spinResults[2] && spinResults[0] == spinResults[1])
            return spinResults[0] is SpinFishType.Blue or SpinFishType.Red? (int)Math.Round(usedPoints *1.5m): usedPoints * 2;
        
        return (int)Math.Round(usedPoints * 1.5d);
    }

    public void SetIsOnScreen()
    {
        _isOnScreen = true;
        _allowNewSpin = true;
    }
    
    public void SetIsOffScreen()
    {
        _isOnScreen = false;
        _allowNewSpin = false;
    }
}

public record SpinRequest(string Username, int PointsUsed);