using Godot;
using Temptic404Overlay.Scripts.Models;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Labels;

public partial class RaidLabel : Label3D
{
    private static string _textToSet = "";
    private static bool _textSet;
    private Label3D _titleLabel;

    public override void _Ready()
    {
        _titleLabel = GetNode<Label3D>("../RaidTitleLabel");
        _titleLabel.Hide(); 
        _textSet = true;
    }

    public override void _Process(double delta)
    {
        if (_textSet) return;
        Text = _textToSet;
        Show();
        _titleLabel.Show();
        _textSet = true;
    }

    public static void OnAlert(object sender, OverlayAlert e)
    {
        GD.Print($"Raid message received: {e.User} ({e.Amount})");
        _textToSet = $"{e.User} ({e.Amount})";
        
        _textSet = false;
    }
}