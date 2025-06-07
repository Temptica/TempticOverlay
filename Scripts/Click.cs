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

		if (OverlayClickModel.Color == Colors.Transparent || OverlayClickModel.Color == Colors.Black)
		{
			var rng = new Random();
			OverlayClickModel.Color = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
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
}
