using Godot;
using Temptica.Overlay.Scripts.Services;

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

    public static void OnAlert(string user, int amount)
    {
        _textToSet = $"{user} ({amount})";
        _textSet = false;
    }
}