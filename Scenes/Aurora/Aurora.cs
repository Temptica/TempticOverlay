using Godot;
using Temptica.Overlay.Scripts;
using Temptica.Overlay.Scripts.SignalR.Listeners;

namespace Temptica.Overlay.scenes.Aurora;

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
			RemainingTime = DurationTime;
		};
	}
}