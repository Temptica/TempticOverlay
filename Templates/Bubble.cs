using System;
using Godot;

namespace Temptic404Overlay.Templates;

public partial class Bubble : RigidBody3D
{
    private Vector3 _position;
    public override void _Ready()
    {
        GlobalRotation = new Vector3(0,0,new Random().Next(0,360));
        BodyEntered += _ =>
        {
            if(new Random().Next(0,20)==0)
                QueueFree();
        };
    }
    
    public override void _PhysicsProcess(double delta)
    {
        //if bubble is above screen, remove it (height is 9)
        if(GlobalPosition.Y > 9f)
            QueueFree();
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
            QueueFree();
            return true;
        }

        return false;
    }
}