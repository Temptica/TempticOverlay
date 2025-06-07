using Godot;
using Temptica.Overlay.Scripts.Models;
using Temptica.Overlay.Scripts.Services;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.Labels;

public partial class LastSubLabel : Label3D
{
    private static string _textToSet = "No Cheers Yet";
    private static bool _textSet;
    
    public override async void _Ready()
    {
        Text = await new ApiService().GetLastSub();
    }
    public override void _Process(double delta)
    {
        if (_textSet) return;
        Text = _textToSet;
        _textSet = true;
    }

    public static void OnAlert(object sender, OverlayAlert e)
    {
        if (e.Type != AlertType.Bit) return;
        _textToSet = $"{e.User} ({e.Amount})";
        _textSet = false;
    }
}