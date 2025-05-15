using System;
using Godot;

namespace Temptica.Overlay.Scripts.Labels;

public partial class ClickCounterDisplay : Node3D
{
	[Export] Label3D _noseLabel;
	[Export] Label3D _bubblesLabel;
	[Export] Label3D _fishesLabel;
	// Called when the node enters the scene tree for the first time.
	
	private static int _bubblesCounter = 0;
	private static int _fishCounter = 0;
	private static int _noseCounter = 0;
	private static bool _shouldUpdate = false;
	
	private static DateTime _lastUpdate = DateTime.MinValue;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_shouldUpdate)
		{
			if (_lastUpdate.AddSeconds(20) < DateTime.Now)
			{
				Hide();
			}
			return;
		}

		if (!IsVisible())
		{
			Show();
		}
		
		_shouldUpdate = false;
		_noseLabel.Text = $"Nose boopes: {_noseCounter}";
		_bubblesLabel.Text = $"Bubbles: {_bubblesCounter}";
		_fishesLabel.Text = $"Fishes: {_fishCounter}";
	}

	public static void UpdateBubbles(int bubblesCount = 1)
	{
		_bubblesCounter += bubblesCount;
		_shouldUpdate = true;
		_lastUpdate = DateTime.Now;
	}

	public static void UpdateFishes(int fishCount = 1)
	{
		_fishCounter += fishCount;
		_shouldUpdate = true;
		_lastUpdate = DateTime.Now;
	}

	public static void UpdateNose()
	{
		_noseCounter++;
		_shouldUpdate = true;
		_lastUpdate = DateTime.Now;
	}
}