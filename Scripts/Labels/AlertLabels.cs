using System;
using System.Linq;
using Godot;
using Temptic404Overlay.Scripts.Models;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Labels;

public partial class AlertLabels : VBoxContainer
{
	
	private static Label _alertTypeLabel;
	private static Label _alertUsernameLabel;
	private static Label _alertMessageLabel;
	private static MeshInstance3D _alertBox;
	
	public override void _Ready()
	{
		var children = GetChildren().Where(c => c is Label).Cast<Label>().ToList();
		_alertTypeLabel = children.First(c => c.Name == "AlertTypeLabel");
		_alertUsernameLabel = children.First(c => c.Name == "AlertUserNameLabel");
		_alertMessageLabel = children.First(c => c.Name == "AlertMessageLabel");
		_alertBox = GetNode<MeshInstance3D>("/root/Overlay/AlertBox");
	}

	public static void OnAlertStart(object sender, OverlayAlert e)
	{
		_alertUsernameLabel.Text = e.User;
		_alertMessageLabel.Text = e.Message;
		_alertTypeLabel.Text = e.Type.ToString();
		_alertBox.Show();
	}
	
	public static void OnAlertEnd(object sender, EventArgs e)
	{
		_alertUsernameLabel.Text = "";
		_alertMessageLabel.Text = "";
		_alertTypeLabel.Text = "";
		_alertBox.Hide();
	}
}
