using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Temptica.Overlay.Scripts.Alerts;

namespace Temptica.Overlay.Scripts.Labels;

public partial class AlertLabels : VBoxContainer
{
	private static Label _alertTypeLabel;
	private static Label _alertUsernameLabel;
	private static Label _alertMessageLabel;
	private static MeshInstance3D _alertBox;
	private static AnimationPlayer _alertAnimationPlayer;
	public Queue<Alert> Alerts = new Queue<Alert>();
	
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
	
	public void OnFollow(Dictionary data)
	{
		Alerts.Enqueue(new FollowAlert("A new otter"));
	}

	public void OnSub(Dictionary data)
	{
		var userName = data["user_name"].AsString();
		var tier = data["tier"].AsString()[0];
		var tts = $"{userName} just subscribed with a tier {tier} sub";
		Alerts.Enqueue(new SubAlert(userName,tts));
	}

	public void OnReSub(Dictionary data)
	{
		var userName = data["user_name"].AsString();
		var message = data["message"].AsString();
		var tier = data["tier"].AsString()[0];
		var cul = data["cumulative_months"].AsStringName();
		var streak = data["streak_months"].AsString();
		var tts = $"{userName} just resubscribed with a tier {tier} sub for a total of {cul} months ";
		if (!string.IsNullOrEmpty(streak))
		{
			tts += $" with a streak of {streak} ";
		}

		if (!string.IsNullOrEmpty(message))
		{
			tts += $". {message.Replace("tempti2","").Trim()}";
		}
		
		Alerts.Enqueue(new ReSubAlert(userName, message,tts));
	}
}
