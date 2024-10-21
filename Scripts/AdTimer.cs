using Godot;
using System;
using System.Threading.Tasks;
using Temptic404Overlay.Scripts;
using Temptic404Overlay.Scripts.SignalR.Listeners;

public partial class AdTimer : Node3D
{
    [Export]
    MeshInstance3D _adMesh = null!;
    [Export]
    Label3D _adLabel = null!;
    
    private DateTime _nextAdTime;
    const string _adText = "Ad in ";
    private bool _isAdPlaying;

    public override void _Ready()
    {
        //listen to signalR
        //first send a request to get the time until the next ad
        
        AdsListener.AdStarted += OnAdStarted;
        AdsListener.AdEnded += OnAdEnded;
        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            await Overlay.SignalRService.RequestAdTime();
        });

    }

    public override void _Process(double delta)
    {
        if(_nextAdTime == default)
        {
            return;
        }
        
        var timeLeft = _nextAdTime - DateTime.Now;
        _adLabel.Text = _adText + (timeLeft.Minutes>0?
            timeLeft.Minutes + "m":
            timeLeft.Seconds >0 && !_isAdPlaying? 
                timeLeft.Seconds + "s":
                "progress");
        
        //set the width and position of the ad bar to show the progress (100% is 60 minutes remaining)
        var percent = (float)(1 - timeLeft.TotalMinutes/60);
        _adMesh.Scale = new Vector3(percent, 1, 1);
        _adMesh.Position = new Vector3(-0.5f + percent/2, 0, 0);
        
    }

    private void OnAdEnded(object sender, DateTime e)
    {
        _nextAdTime = e;
        _isAdPlaying = false;
    }

    private void OnAdStarted(object sender, EventArgs e)
    {
        _isAdPlaying = true;
        
    }
}
