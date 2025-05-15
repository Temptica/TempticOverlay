using System;

namespace Temptica.Overlay.Scripts.Extensions;

public static class StringExtension
{
    public static string CleanEmoteName(this string message)
    {
        return message.Replace("tempti2", "", StringComparison.OrdinalIgnoreCase);
    }
}