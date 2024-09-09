using Godot;

namespace Temptic404Overlay.Scripts.Models;

public class OverlayClickModel
{
    public string X { get; set; }
    public string Y { get; set; }
    public string Username { get; set; }
    public Color Color { get; set; }
    
    public OverlayClickModel(string x, string y, string username, string color)
    {
        X = x;
        Y = y;
        Username = username;
        Color = new Color(color);
    }
    
}