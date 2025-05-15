using Godot;

namespace Temptica.Overlay.Scripts.Spawners;

public partial class Plushie : RigidBody3D
{
    [Export] public double Lifetime = 15f;

    public override void _Process(double delta)
    {
        Lifetime -= delta;

        if (Lifetime is < 14f and > 13f)
        { 
            SetCollisionMaskValue(12, true);
        }

        if (Lifetime <= 0)
        {
            QueueFree();
        }
    }
}