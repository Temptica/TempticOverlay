using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SlotMachine : Node3D
{
    [Export]
    private SlotSpinner _leftSpinner;
    [Export]
    private SlotSpinner _centerSpinner;
    [Export]
    private SlotSpinner _rightSpinner;
    private KeyValuePair<string,int>? _currentSpin;
    
    private static Dictionary<string,int> spinQueue = new Dictionary<string, int>();
    
    public override void _Ready()
    {
        //signalR
    }
    
    public static void Spin(string username, int betAmount)
    {
        spinQueue.TryAdd(username, betAmount);
    }
    
    public override void _Process(double delta)
    {
        if(_currentSpin is null && spinQueue.Count == 0)
        {
            return;
        }

        if (_currentSpin is not null)
        {
            if(_leftSpinner.IsSpinning() || _centerSpinner.IsSpinning() || _rightSpinner.IsSpinning())
            {
                return;
            }
        }
        
        if(spinQueue.Count > 0 && _currentSpin is null)
        {
            var spin = spinQueue.First();
            spinQueue.Remove(spin.Key);
            _currentSpin = spin;
            _leftSpinner.Spin();
            _centerSpinner.Spin();
            _rightSpinner.Spin();
        }
        
        if(_currentSpin is null)
        {
            return;
        }
    }
    
}
