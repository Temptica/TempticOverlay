using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using TwitcherSharp;
using TwitcherSharp.Api.Generated;
using TwitcherSharp.Api.Generated.Users;

namespace Temptica.Overlay.Scripts.Services;

public partial class UserService : Node
{
    private static TwitchApi TwitchApi => TwitchApi.Instance;
    private GDScript _getUsersObject;
    public static UserService Instance { get; private set; }

    public List<User> Users { get; } = [];

    public override void _Ready()
    {
        Instance = this;
        _getUsersObject = GD.Load<GDScript>("res://addons/twitcher/generated/twitch_get_users.gd");
    }

    public User GetUser(string id)
    {
        return Users.FirstOrDefault(x => x.Id == id);
    }

    public async Task<User> GetOrCreateUser(string id)
    {
        return GetUser(id) ?? await CreateUser(id);
    }

    public async Task<User> CreateUser(string id)
    {
        var userData = await TwitchService.Instance.GetUserById(id);
        var colorData = (await TwitchApi.GetUserChatColor([id])).Data[0];
        var user = new User
        {
            Id = id,
            Username = userData.DisplayName,
            Color = colorData.Color is not null ? Color.FromHtml(colorData.Color) : GenerateVibrantColor(int.Parse(id))
        };
        Users.Add(user);
        return user;
    }

    /// <summary>
    /// Generates a consistent, visually vibrant Color object for a given user ID.
    /// Uses HSL for better distribution and avoids dark/light colors.
    /// </summary>
    /// <param name="userId">The integer ID of the user.</param>
    /// <returns>A Godot.Color object.</returns>
    public static Color GenerateVibrantColor(int userId)
    {
        var hue = (float)(userId % 360);
        const float saturation = 0.7f;
        const float lightness = 0.5f;
        var hNorm = hue / 360.0f;
        return HslToRgb(hNorm, saturation, lightness);
    }

    // Manual HSL to RGB Conversion Helper (Returns a Godot.Color)
    private static Color HslToRgb(float h, float s, float l)
    {
        h = Mathf.Clamp(h, 0.0f, 1.0f);
        s = Mathf.Clamp(s, 0.0f, 1.0f);
        l = Mathf.Clamp(l, 0.0f, 1.0f);

        if (s == 0.0f)
        {
            // Achromatic (grey)
            return new Color(l, l, l);
        }

        var q = l < 0.5f ? l * (1 + s) : l + s - l * s;
        var p = 2 * l - q;

        var r = HueToRgb(p, q, h + 1.0f / 3.0f);
        var g = HueToRgb(p, q, h);
        var b = HueToRgb(p, q, h - 1.0f / 3.0f);

        return new Color(r, g, b);
    }

    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0.0f) t += 1.0f;
        if (t > 1.0f) t -= 1.0f;

        return t switch
        {
            < 1.0f / 6.0f => p + (q - p) * 6.0f * t,
            < 1.0f / 2.0f => q,
            < 2.0f / 3.0f => p + (q - p) * (2.0f / 3.0f - t) * 6.0f,
            _ => p
        };
    }
}

public class User
{
    public string Username { get; set; }
    public string Id { get; set; }
    public Color Color { get; set; }
}