using Godot;

namespace Temptica.Overlay.Scripts.Models;

public record OverlayClickModel(float X, float Y, string Username, string UserId ,Color Color)
{
    public bool Anonymous => Username == "Anonymous";
    public override string ToString()
    {
        return "X: " + X + "\nY: " + Y + "\nUsername: " + Username + "\nColor: " + Color;
    }
}