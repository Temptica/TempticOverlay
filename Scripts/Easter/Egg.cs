using System;
using Godot;
using Temptica.Overlay.Scripts.Models;

namespace Temptica.Overlay.Scripts.Easter;

public partial class Egg : RefCounted
{
    private const string EggPath = "res://scene_nexus/eastern_egg/eastern_egg.tscn";
    protected static PackedScene _eggScene;
    protected Node3D Data;

    public string Name => EggData.Name;
    public EggMetaData EggData { get; set; }

    public bool IsHit = false;

    public static Egg Create(Node3D parent, EggMetaData eggData)
    {
        _eggScene ??= GD.Load<PackedScene>(EggPath);

        var egg = _eggScene.Instantiate<Node3D>();
        parent.AddChild(egg);
        var cSharpEgg = new Egg();
        cSharpEgg.EggData = eggData;
        cSharpEgg.Data = egg;
        egg.Set("title", eggData.Name);

        var image = new Image();
        image.LoadPngFromBuffer(Convert.FromBase64String(eggData.Data.Image.Split(',')[1]));
        egg.Set("image", ImageTexture.CreateFromImage(image));
        egg.Set(RigidBody3D.PropertyName.GravityScale, 0.0f);
        return cSharpEgg;
    }

    public static Egg FromNode(Node3D eggNode)
    {
        var eggData = eggNode?.GetMeta("_egg").AsGodotObject() as Egg;
        return eggData;
    }

    public bool CheckHit(Vector2 position)
    {
        if (IsHit) return false;

        var pos = new Vector2(Data.GlobalTransform.Origin.X, Data.GlobalTransform.Origin.Y);
        IsHit = position.DistanceTo(pos) <= 2f;
        return IsHit;
    }

    public void SetGlobalPosition(Vector3 vector3)
    {
        Data.SetGlobalPosition(vector3);
    }

    public void Remove()
    {
        Data.QueueFree();
        Data = null;
        Unreference();
    }
}