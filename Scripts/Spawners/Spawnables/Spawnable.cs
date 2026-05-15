using Godot;

namespace Temptica.Overlay.Scripts.Spawners.Spawnables;

public abstract partial class Spawnable : RigidBody3D
{
    [Export] private double _lifeTime = 15;

    public override void _Process(double delta)
    {
        _lifeTime -= delta;
        if (_lifeTime < 0)
        {
            QueueFree();
        }
    }
    
}