using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Models;

public record OverlayAlert(string User, string Message, AlertType Type, int Amount);