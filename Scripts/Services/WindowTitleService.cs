using Godot;

namespace Temptica.Overlay.Scripts.Services;

public partial class WindowTitleService : Node
{
    private string _mainTitle;

    public override void _EnterTree()
    {
        _mainTitle = ProjectSettings.GetSetting("application/config/name").AsString();
        GetTree().NodeAdded += OnNodeAdded;
    }

    public override void _ExitTree()
    {
        GetTree().NodeAdded -= OnNodeAdded;
    }

    private void OnNodeAdded(Node node)
    {
        if (node is not Window window || window == GetTree().Root) return;
        if (!string.IsNullOrEmpty(window.Title) && window.Title != _mainTitle) return;

        window.Title = $"{_mainTitle} - {window.Name}";
    }
}
