using Godot;
using Temptica.Overlay.Scripts.Services;
using Temptica.Overlay.Scripts.SignalR.Listeners;

namespace Temptica.Overlay.Scripts.Labels;

public partial class FirstViewerLabel : Label3D
{
    private static string _textToSet = "Be the first!";
    private static bool _textSet = false;
    
    public override async void _Ready()
    {
        SetFirstListener.SetFirst += SetFirst;
        Text = await new ApiService().GetFirstViewer();
    }
    public override void _Process(double delta)
    {
        if (_textSet) return;
        Text = _textToSet;
        _textSet = true;
    }

    private void SetFirst(object sender, string e)
    {
        _textToSet = e;
        _textSet = false;
    }
}