using Godot;

namespace Temptica.GreenHeatSharp;

public static class InputEventMouseButtonExtensions
{
    extension(InputEventMouseButton button)
    {
        public GreenHeatMessage AsGreenHeatMessage()
        {
            return !button.HasMeta("greenheat") ? null : button.GetMeta("greenheat").As<GreenHeatMessage>();
        }
    }
}