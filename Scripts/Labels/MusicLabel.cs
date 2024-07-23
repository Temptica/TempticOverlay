using Godot;
using System;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptic404Overlay.Scripts.Spotify;

public partial class MusicLabel : Label
{
	private string changedText = "";
	private bool textChanged = false;
	public override void _Ready()
	{
		SpotifyService.SongChanged += OnSongChanged;
	}

	private void OnSongChanged(object sender, string e)
	{
		changedText = e;
		textChanged = true;
	}

	public override void _Process(double delta)
	{
		if (!textChanged) return;
		Text = changedText;
		textChanged = false;
	}
}
