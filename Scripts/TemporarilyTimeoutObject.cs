using Godot;

namespace Temptic404Overlay.Scripts;

public partial class TemporarilyTimeoutObject<T>: Node3D where T : Node3D
{
    protected double RemainingTime;
    protected T Target;

    public override void _Process(double delta)
    {
        if (RemainingTime > 0)
        {
            RemainingTime -= delta;
            if(!Target.IsVisible())
            {
                Target.Visible = true;
            }
        }
        else if(Target.IsVisible())
        {
            Target.Visible = false;
        }
    }
    
}