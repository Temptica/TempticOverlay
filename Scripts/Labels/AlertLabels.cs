using System.Linq;
using Godot;
using Temptica.Overlay.Scripts.Alerts;

namespace Temptica.Overlay.Scripts.Labels;

public partial class AlertLabels : VBoxContainer
{
	
	private static Label _alertTypeLabel;
	private static Label _alertUsernameLabel;
	private static Label _alertMessageLabel;
	private static MeshInstance3D _alertBox;
	private static AnimationPlayer _alertAnimationPlayer;
	
	public override void _Ready()
	{
		var children = GetChildren().Where(c => c is Label).Cast<Label>().ToList();
		_alertTypeLabel = children.First(c => c.Name == "AlertTypeLabel");
		_alertUsernameLabel = children.First(c => c.Name == "AlertUserNameLabel");
		_alertMessageLabel = children.First(c => c.Name == "AlertMessageLabel");
		_alertBox = GetNode<MeshInstance3D>("%AlertBox");
		_alertAnimationPlayer = GetNode<AnimationPlayer>("%AlertBoxAnimationPlayer");
	}

	public static void ShowAlert(Alert e)
	{
		_alertUsernameLabel.Text = e.User;
		_alertMessageLabel.Text = e.Message;
		_alertTypeLabel.Text = e.Event.ToString();
		_alertBox.Show();
		_alertAnimationPlayer.Play("ShowAlertBox");
	}
	
	public static void HideAlert()
	{
		_alertAnimationPlayer.Play("HideAlertBox");
	}
}
