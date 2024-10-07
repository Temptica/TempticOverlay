using Godot;
using System;
using Temptic404Overlay.Templates;
using Temptica.TwitchBot.Shared.enums;

public partial class BattleFish : RigidBody3D
{
    public FishColor Color { get; private set; }
    public override void _Ready()
    {
        LinearVelocity = new Vector3((float)new Random().NextDouble(), (float)new Random().NextDouble(), 0);
    }
    
    public void SetFishColor(FishColor color)
    {
        Color = color;
        var sprite = new Sprite3D();
        sprite.Texture = GD.Load<Texture2D>("res://Images/fish" + color + ".png");
        AddChild(sprite);
        sprite.PixelSize = 0.006f;
    }
    
    public void _on_body_entered(Node body)
    {
        if (body is not BattleFish fish || fish.Color == Color) return;
        
        if (new Random().Next(0, 4) == 0)
        {
            QueueFree();
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if(LinearVelocity.Length() < 0.3f)
        {
            //Always go in teh same direction the fish is already going so it doesn't weirdly turn backwards
            var x = LinearVelocity.X;
            var y = LinearVelocity.Y;

            if (x >= 0)
            {
                x += 0.1f * (float)delta;
            }
            else
            {
                x -= 0.1f * (float)delta;
            }
			
            if(y>=0)
            {
                y += 0.1f * (float)delta;
            }
            else
            {
                y -= 0.1f * (float)delta;
            }
			
            LinearVelocity = new Vector3(x, y, 0);
        }
    }
}
