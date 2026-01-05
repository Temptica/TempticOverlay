using Temptica.Overlay.Enums;

namespace Temptica.Overlay.Scripts.Models;

public record OverlayAlert(string User, string Message, AlertType Type, int Amount);