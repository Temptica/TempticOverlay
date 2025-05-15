using System;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts;

public partial class AdTimer : Node3D
{
    [Export] MeshInstance3D _adMesh = null!;
    [Export] Label3D _adLabel = null!;

    private DateTime _nextAdTime;
    private const string AdsText = "Ad in ";
    private bool _isAdPlaying;
    private bool _playedBell = false;
    private TimeSpan _adsTimeLeft;

    private const int AdsDuration = 180;
    private const int AdsStartingSoonWarning = 30;

    public override void _Ready()
    {
        //listen to signalR
        //first send a request to get the time until the next ad

        AdsListener.AdStarted += OnAdStarted;
        AdsListener.AdEnded += OnAdEnded;
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            await Overlay.SignalRService.RequestAdTime();
        });
    }

    public override void _Process(double delta)
    {
        if (_nextAdTime == default)
        {
            return;
        }

        var timeLeft = _nextAdTime.TimeOfDay - DateTime.UtcNow.TimeOfDay;
        if (!_isAdPlaying)
        {
            if (timeLeft.Hours > 0) _adLabel.Text = $"{AdsText} {timeLeft.Hours}h and {timeLeft.Minutes}m";
            else if (timeLeft.Minutes > 0) _adLabel.Text = AdsText + timeLeft.Minutes + "m";
            else if (timeLeft.Seconds > 0)
            {
                _adLabel.Text = AdsText + timeLeft.Seconds + "s";
                if (timeLeft.Seconds <= AdsStartingSoonWarning && !_playedBell)
                {
                    AudioPlayer.PlayAudio(AudioEffects.Bell);
                    Overlay.SignalRService.SendChatMessage(
                        "BONG! Ads will start in approximately 30 second! Get yourself some snacks " +
                        "and water and we'll see you after!");
                    //TODO: Start random overlay game;
                    _playedBell = true;
                }
            }
        }
        else
        {
            _adsTimeLeft -= TimeSpan.FromSeconds(delta);
            if (_adsTimeLeft.Seconds > 0)
            {
                _adLabel.Text = "Ads remaining: " + (_adsTimeLeft.Minutes > 0
                    ? _adsTimeLeft.Seconds > 30
                        ? _adsTimeLeft.Minutes + 1 + "m"
                        : _adsTimeLeft.Minutes + "m"
                    : _adsTimeLeft.Seconds + "s");
            }
        }

        //set the width and position of the ad bar to show the progress (100% is 60 minutes remaining)
        var mesh = (BoxMesh)_adMesh.Mesh;
        var percent = 0f;

        if (_isAdPlaying)
        {
            percent = (float)_adsTimeLeft.TotalSeconds / AdsDuration;
            _playedBell = false;
        }
        else if (timeLeft.Seconds > 0 || timeLeft.Minutes > 0)
        {
            percent = 1 - (float)Mathf.Min(timeLeft.TotalSeconds, 3600) / 3600;
        }
        else
        {
            _adLabel.Text = "Waiting for ads to start any moment...";
        }

        percent = Math.Clamp(percent, 0, 1);

        const int originalSize = 4;
        mesh.Size = new Vector3(originalSize * percent, mesh.Size.Y, mesh.Size.Z);

        _adMesh.Position = new Vector3((mesh.Size.X - originalSize) / 2f, 0, 0);
    }

    private void OnAdEnded(object sender, DateTime e)
    {
        GD.Print($"next ad at {e}");
        _nextAdTime = e;
        _isAdPlaying = false;
    }

    private void OnAdStarted(object sender, EventArgs e)
    {
        _isAdPlaying = true;
        _adsTimeLeft = TimeSpan.FromSeconds(AdsDuration);
        _nextAdTime = DateTime.Now;
    }
}