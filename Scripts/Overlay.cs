using System.Linq;
using Godot;
using Temptic404Overlay.Scripts.SignalR;

namespace Temptic404Overlay.Scripts;

public partial class Overlay : Node3D
{
	private SignalRService _signalRService;

	public override void _Ready()
	{
		_signalRService = new SignalRService();
	}


	public override async void _ExitTree()
	{
		await _signalRService.DisposeAsync();
	}
}
