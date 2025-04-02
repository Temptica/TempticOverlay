using System;
using Godot;
using Temptic404Overlay.Scripts;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Templates;

public partial class Bubble : RigidBody3D
{
    private Vector3 _position;
    [Export] public float InitialX;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] public float Amplitude = 0.75f;
    [Export] public float Frequency = 0.25f;
	
    [Export] private float _fallSpeedMin = 0.95f;
    [Export] private float _fallSpeedMax = 1f;
    private float _time;
    public override void _Ready()
    {
        //GlobalRotation = new Vector3(0,0,new Random().Next(0,360));
        Scale = Vector3.One * new Random().Next(75, 126) / 100;
        BodyEntered += _ =>
        {
            if(new Random().Next(0,20)==0)
                QueueFree();
        };
        
    }
    
    public override void _PhysicsProcess(double delta)
    {
        _time += (float)delta;
        //if bubble is above screen, remove it (height is 9)
        if(GlobalPosition.Y > 9f)
            QueueFree();
        
        var xPos = Mathf.Sin(_time * Frequency) * Amplitude + InitialX;
		
        GlobalPosition = new Vector3(xPos, GlobalPosition.Y , GlobalPosition.Z);
        _position = GlobalPosition;
    }
    
    //check click
    public bool CheckClick(Vector3 clickPos)
    {
        var distance = _position.DistanceTo(clickPos);
        GD.Print("Distance: " + distance);
        GD.Print(_position);
        GD.Print(clickPos);
        if(distance < 0.5f)
        {
            AudioPlayer.PlayAudio(AudioEffects.Pop);
            QueueFree();
            return true;
        }

        return false;
    }
}