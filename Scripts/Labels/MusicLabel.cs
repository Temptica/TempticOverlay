using Godot;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.Labels;

public partial class MusicLabel : Label3D
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
