using System;
using Godot;
using Temptica.Overlay.Scripts.Models;

namespace Temptica.Overlay.Scripts.Easter;

public partial class DisplayEgg : Egg
{
    private const string EggPath = "res://Scenes/display_eastern_egg.tscn";

    public new static DisplayEgg Create(Node3D parent, EggMetaData eggData)
    {
        _eggScene ??= GD.Load<PackedScene>(EggPath);

        var egg = _eggScene.Instantiate<Node3D>();
        parent.AddChild(egg);
        var cSharpEgg = new DisplayEgg();
        cSharpEgg.EggData = eggData;
        cSharpEgg.Data = egg;
        egg.Set("title", eggData.Name);

        var image = new Image();
        image.LoadPngFromBuffer(Convert.FromBase64String(eggData.Data.Image.Split(',')[1]));
        egg.Set("image", ImageTexture.CreateFromImage(image));
        egg.Set(RigidBody3D.PropertyName.GravityScale, 0.0f);
        
        var label = egg.GetNode<Label3D>("Label3D");
        label.SetText(eggData.Name);
        
        //despawn after 30 seconds
        
        return cSharpEgg;
    }
}