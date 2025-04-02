using Godot;
using System;
using Temptic404Overlay.Scripts;
using Temptic404Overlay.Scripts.SignalR.Listeners;

public partial class Aurora : TemporarilyTimeoutObject<MeshInstance3D>
{
	private const int DurationTime = 120;
	
	[Export] private MeshInstance3D _mesh;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Target = _mesh;
		NorthPoleLightListener.Lights += (_, _) =>
		{
			GD.Print("Lights on");
			RemainingTime = DurationTime;
		};
	}
}
