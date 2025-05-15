using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Temptica.GodotExtensions;
using Temptica.Overlay.Scripts.Extensions;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.Easter;

public partial class EggDisplay : Node3D
{
    private float _remainingTime = 10f;
    [Export] private Label3D _nameLabel;
    private PackedScene _eggScene;
    List<Egg> _eggs = [];
    
    public override void _Ready()
    {
        _eggScene = GD.Load<PackedScene>("res://scenes/easter/egg.tscn");
        var offset = new Vector3(0.2f, 0,0);
        GD.Print(_nameLabel?.Text);
        _nameLabel ??= GetChildren().OfType<Label3D>().FirstOrDefault();

        if (_nameLabel == null)
        {
            _nameLabel = new Label3D()
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            AddChild(_nameLabel);
            _nameLabel.Text = "Egg";
        }
        
        GD.Print(_nameLabel.Text);
        
        foreach (EggType eggType in Enum.GetValues(typeof(EggType)))
        {
            GD.Print($"Egg type: {eggType}");
            var egg = _eggScene.Instantiate() as Egg;
            egg!.SetSprite(eggType);
            egg.AnimatedSprite3D.Modulate = Colors.Transparent;
            egg.Position = offset;
            _eggs.Add(egg);
            offset = new Vector3(offset.X+0.4f, 0,0);
            AddChild(egg);
        }
        
        _nameLabel.Hide();
        
        EggStatsListener.OnEggStatsReceived += (_, tuple) =>
        {
            _nameLabel.CallAsync(Label3D.MethodName.SetText, tuple.Name);
            _nameLabel.CallAsync(Node3D.MethodName.Show);
            _remainingTime = 10f;
            
            GD.Print(_eggs.Count.ToString());

            var eggsToSkip = new List<Egg>();

            foreach (var (type, _) in tuple.Eggs)
            {
                var egg = _eggs.FirstOrDefault(e => e.EggType == type);
                
                egg?.AnimatedSprite3D.CallAsync("set_modulate", Colors.White);
                eggsToSkip.Add(egg);
            }

            foreach(var egg in _eggs.Where(egg => !eggsToSkip.Contains(egg)))
            {
                egg?.AnimatedSprite3D.CallAsync("set_modulate", Color.FromHtml("2f2f2f"));
            }
        };
    }

    public override void _Process(double delta)
    {
        _remainingTime -= (float)delta;

        if (!(_remainingTime <= 0f)) return;
        
        if (_eggs == null || _eggs.Count == 0)
        {
            GD.Print("No eggs received");
            return;
        }
        
        _eggs.ForEach(egg => egg.AnimatedSprite3D.Modulate = Colors.Transparent);
        _nameLabel.Hide();
    }
}