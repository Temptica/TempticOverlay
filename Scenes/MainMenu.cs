using Godot;

namespace Temptica.Overlay.Scenes;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
		CameraServer.MonitoringFeeds = true;
	}

	private void NormalPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/overlay.tscn");
	}
	private void CoStreamPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/co_host_overlay.tscn");
	}
}