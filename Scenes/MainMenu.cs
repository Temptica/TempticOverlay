using Godot;

namespace Temptica.Overlay.scenes;

public partial class MainMenu : Control
{
	private void NormalPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/overlay.tscn");
	}
	private void CoStreamPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/co_host_overlay.tscn");
	}
}