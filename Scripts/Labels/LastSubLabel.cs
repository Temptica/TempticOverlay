using Godot;
using Temptic404Overlay.Scripts.Models;
using Temptic404Overlay.Scripts.Services;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Labels;

public partial class LastSubLabel : Label3D
{
    private static string _textToSet = "No Cheers Yet";
    private static bool _textSet = false;
    
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