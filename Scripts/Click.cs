using System;
using System.Globalization;
using System.Linq;
using Godot;
using Temptica.Overlay.Scripts.Models;

namespace Temptica.Overlay.Scripts;

public partial class Click : Node3D
{
	public OverlayClickModel OverlayClickModel { get; set; }
	private MeshInstance3D _meshInstance;
	private int _pointsToAdd;
	
	public override void _Ready()
	{
		var children = GetChildren();
		var label = (Label3D)children.First(c=>c is Label3D);
		
		var mesh = (MeshInstance3D)children.First(c=>c is MeshInstance3D);
		mesh = (MeshInstance3D)mesh.Duplicate();
		_meshInstance = mesh;
		
		GD.Print(OverlayClickModel);

		if (OverlayClickModel.Color == default || OverlayClickModel.Color == Colors.Transparent || OverlayClickModel.Color == Colors.Black || OverlayClickModel.Color == Colors.White)
		{
			OverlayClickModel.Color = GenerateVibrantColor(OverlayClickModel.Username.GetHashCode());
		}
		
		label!.Text = OverlayClickModel.Username;
		label.Modulate = OverlayClickModel.Color;
		var material = new StandardMaterial3D();
		material.AlbedoColor = OverlayClickModel.Color;
		material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
		_meshInstance.Mesh.SurfaceSetMaterial(0, material);
		
		// left under =0,0,0 top right = 16,9,0
		var x = float.Parse(OverlayClickModel.X, CultureInfo.InvariantCulture);
		var y = float.Parse(OverlayClickModel.Y, CultureInfo.InvariantCulture);
		
		//invert the Y value (0 is 1 and 1 is 0)
		GlobalPosition = new Vector3(16 * x, 9 - 9 * y, 0);
		
		var tweenLabel = GetTree().CreateTween();
		var tweenMesh = GetTree().CreateTween();
		
		tweenMesh.TweenProperty(material, "albedo_color:a", 0,4).SetTrans(Tween.TransitionType.Cubic).SetDelay(1);
		tweenLabel.TweenProperty(label, "modulate:a", 0,4).SetTrans(Tween.TransitionType.Cubic).SetDelay(1);
		tweenLabel.TweenCallback(Callable.From(QueueFree));
	}

	public override void _Process(double delta)
	{
		if (_pointsToAdd > 0)
		{
			GetChildren().OfType<Label3D>().First().Text += $" +{_pointsToAdd}" ;
			_pointsToAdd = 0;
		}
	}

	public void AddPoints(int points)
	{
		_pointsToAdd = points;
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
        var saturation = 0.7f;
        var lightness = 0.5f;
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
        
        var r = HueToRgb(p, q, h + 1.0f/3.0f);
        var g = HueToRgb(p, q, h);
        var b = HueToRgb(p, q, h - 1.0f/3.0f);

        return new Color(r, g, b, 1.0f); 
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
