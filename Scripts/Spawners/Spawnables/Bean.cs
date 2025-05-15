using Godot;

namespace Temptica.Overlay.Scripts.Spawners.Spawnables;

public partial class Bean : RigidBody3D
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