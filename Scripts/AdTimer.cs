using System;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.SignalR.Listeners;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts;

public partial class AdTimer : Node3D
{
    [Export]
    MeshInstance3D _adMesh = null!;
    [Export]
    Label3D _adLabel = null!;
    
    private DateTime _nextAdTime;
    const string _adText = "Ad in ";
    private bool _isAdPlaying;
    private bool _playedBell = false;

    public override void _Ready()
    {
        //listen to signalR
        //first send a request to get the time until the next ad
        
        AdsListener.AdStarted += OnAdStarted;
        AdsListener.AdEnded += OnAdEnded;
        _ = Task.Run(async () =>
        {
            GD.Print("Requesting ad time in t-2 seconds");
            await Task.Delay(2000);
            GD.Print("Requesting ad time");
            await Overlay.SignalRService.RequestAdTime();
        });
    }

    public override void _Process(double delta)
    {
        if(_nextAdTime == default)
        {
            return;
        }
        
        var timeLeft = _nextAdTime.TimeOfDay - DateTime.UtcNow.TimeOfDay;
        
        if (timeLeft.Hours > 0) _adLabel.Text = $"{_adText} {timeLeft.Hours}h and {timeLeft.Minutes}m";
        else if (timeLeft.Minutes > 0 && !_isAdPlaying) _adLabel.Text = _adText + timeLeft.Minutes + "m";
        else if (timeLeft.Seconds > 0 && !_isAdPlaying)
        {
            _adLabel.Text = _adText + timeLeft.Seconds + "s";
            if (timeLeft.Seconds <= 30 && !_playedBell)
            {
                AudioPlayer.PlayAudio(AudioEffects.Bell);
                _playedBell = true;
            }
        }
        else
        {
            var adsLeft = TimeSpan.FromSeconds(180) + timeLeft;
            if (adsLeft.Seconds > 0)
            {
               _adLabel.Text = "Ads remaining: " + (adsLeft.Minutes > 0
                                   ? adsLeft.Seconds>30
                                       ? adsLeft.Minutes+1 + "m" 
                                       : adsLeft.Minutes + "m"
                                   : adsLeft.Seconds + "s");
            }
        }
        
        //set the width and position of the ad bar to show the progress (100% is 60 minutes remaining)
        var mesh = (BoxMesh)_adMesh.Mesh;
        var percent =0f ;
        
        if(_isAdPlaying)
        {
            var adsLeft = TimeSpan.FromSeconds(180) + timeLeft;
            GD.Print(adsLeft);
            percent = (float)adsLeft.TotalSeconds / 180f; 
            _playedBell = false;
        }
        else if (timeLeft.Seconds > 0 || timeLeft.Minutes > 0)
        {
            percent = 1 - (float)timeLeft.TotalSeconds / 3600f;
            percent = Math.Clamp(percent, 0, 1);
        }
        else
        {
            _adLabel.Text = "Waiting for ads to start any moment...";
        }
        const int originalSize = 4;
        mesh.Size = new Vector3(originalSize*percent, mesh.Size.Y, mesh.Size.Z);

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
        _nextAdTime = DateTime.Now;
    }
}