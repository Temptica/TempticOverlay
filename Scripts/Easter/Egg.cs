using Godot;
using System;
using Temptica.TwitchBot.Shared.enums;

public partial class Egg : Node3D
{
    [Export] public AnimatedSprite3D AnimatedSprite3D = null!;

    public EggType EggType { get; set; }

    public void SetSprite(EggType eggType)
    {
        AnimatedSprite3D.Frame = (int)eggType;
        EggType = eggType;
    }

    public bool IsHit(Vector2 position)
    {
        var pos = new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Y);
        return position.DistanceTo(pos) <= 0.4f;
    }
}