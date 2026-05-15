using System;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Enums;
using TwitcherSharp.Api.Generated;
using TwitcherSharp.Chat;
using TwitcherSharp.EventSub;
using TwitcherSharp.EventSub.Generated.ChannelAdBreakBegin;

namespace Temptica.Overlay.Scripts;

public partial class AdTimer : Node3D
{
    [Export] private MeshInstance3D _adMesh = null!;
    [Export] private Label3D _adLabel = null!;

    private DateTime _nextAdTime;
    private const string AdsText = "Ad in ";
    private bool _isAdPlaying;
    private bool _playedBell;
    private TimeSpan _adsTimeLeft;
    private bool _firstLoad = true;

    private const int AdsDuration = 180;
    private const int AdsStartingSoonWarning = 30;


    public override void _Ready()
    {
        
        var listener = TwitchEventListener<TwitchChannelAdBreakBeginEvent>
            .FromObject(GetNode("AdsStartedEventListener"));
        listener.Received += OnAdStarted;
        _ = GetNextAdTime().ConfigureAwait(0);
    }

    private void OnAdStarted(TwitchChannelAdBreakBeginEvent e)
    {
        _isAdPlaying = true;
        _adsTimeLeft = TimeSpan.FromSeconds(AdsDuration);
        _nextAdTime = DateTime.Now;
    }

    private async Task GetNextAdTime()
    {
        if (_firstLoad) await Task.Delay(5000);
        else _firstLoad = false;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        var result = await TwitchApi.Instance.GetAdSchedule(Overlay.BroadcasterId);

        _nextAdTime = DateTime.UnixEpoch.AddSeconds(result.Data[0].NextAdAt);
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
                    _playedBell = true;
                    _ = TwitchBot.SendMessage(
                        "BONG! Ads will start in approximately 30 second! Get yourself some snacks " +
                        "and water and we'll see you after!");
                }
            }
        }
        else
        {
            _adsTimeLeft -= TimeSpan.FromSeconds(delta);
            if (_adsTimeLeft.Seconds > 0 || _adsTimeLeft.Minutes > 0)
            {
                _adLabel.Text = "Ads remaining: " + (_adsTimeLeft.Minutes > 0
                    ? _adsTimeLeft.Seconds > 30
                        ? _adsTimeLeft.Minutes + 1 + "m"
                        : _adsTimeLeft.Minutes + "m"
                    : _adsTimeLeft.Seconds + "s");
            }
            else
            {
                _isAdPlaying = false;
                _playedBell = false;
                _adLabel.Text = "Ads ended!";
                _ = GetNextAdTime();
            }
        }

        //set the width and position of the ad bar to show the progress (100% is 60 minutes remaining)
        var mesh = (BoxMesh)_adMesh.Mesh;
        var percent = 0f;

        if (_isAdPlaying)
        {
            percent = (float)_adsTimeLeft.TotalSeconds / AdsDuration;
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
}