using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.Alerts;
using Temptic404Overlay.Scripts.Services;
using Temptic404Overlay.Scripts.Spotify;

namespace Temptic404Overlay.Scripts.LaserShow;

public partial class LaserShow : Node3D
{
    private ShowLoader _showLoader;
    private PackedScene _laserScene;
    private AudioListener _audioListener;
    private Laser[] _lasers;
    public bool IsPlaying { get; private set; }
    public override void _Ready()
    {
        _laserScene = GD.Load<PackedScene>("res://Templates/laser.tscn");
        _audioListener = GetNode<AudioListener>("%AudioListener");
        BitsAlert.StartShow += async void (sender, song) =>
        {
            await StartShow(song);
        };
    }

    private async Task StartShow(string song)
    {
        if (IsPlaying)
        {
            return;
        }
        
        IsPlaying = true;
        
        try
        {
            await SpotifyService.PlaySong(_showLoader.SongId);
        }
        catch (Exception)
        {
            // ignored
        }
        
        try
        {
            await SpotifyService.Skip();
        }
        catch (Exception)
        {
            // ignored
           
        }
        
        await Task.Delay(800);
        
        _showLoader = ShowLoader.Load($@"H:\Projects\Twitch\LaserShows\{song}.json");
        
        var tweens = new Tween[_showLoader.LaserCount];
        
        _lasers = new Laser[_showLoader.LaserCount];
        var lastTweenEndTime = new Dictionary<int, float>();
        
        GD.Print($"Loading show {_showLoader.SongName} with {_showLoader.LaserCount} lasers and {_showLoader.LaserGroupCount} groups.");

        foreach (var group in _showLoader.LaserGroups)
        {
            foreach (var laser in group.Lasers)
            {
                var laserInstance = _laserScene.Instantiate<Laser>();
                CallThreadSafe("add_child", laserInstance);
                laserInstance.CallThreadSafe("set_global_position", group.Position);
                laserInstance.GetMaterial().CallDeferred("set_albedo", new Color("00000000"));
                
                _lasers[laser.LaserId] = laserInstance;
                tweens[laser.LaserId] = GetTree().CreateTween();
                lastTweenEndTime.Add(laser.LaserId, 0f);
                
                GD.Print($"Laser {laser.LaserId} loaded");
            }
        }
        GD.Print("Loading tweens");
        var counter = 0;
       
        
        foreach (var keyFrame in _showLoader.LaserKeyFrames)
        {
            GD.Print($"Loaded {++counter}/{_showLoader.LaserKeyFrames.Count}");
            var lastTweenEnd = lastTweenEndTime[keyFrame.LaserId];
            
            var laser = _lasers[keyFrame.LaserId];
            
            var tween = tweens[keyFrame.LaserId];
            if (laser.RotationDegrees != keyFrame.Rotation)
            {
                tween
                    .Parallel()
                    .TweenProperty(laser, "rotation_degrees", keyFrame.Rotation, keyFrame.Duration)
                    .SetDelay(keyFrame.Time-lastTweenEnd);
            }

            if (!keyFrame.On)
            {
                tween
                    .Parallel()
                    .TweenProperty(laser.GetMaterial(), "albedo_color", new Color("00000000"), keyFrame.Duration)
                    .SetDelay(keyFrame.Time-lastTweenEnd);
            }
            else if (laser.GetMeshColor() != new Color(keyFrame.Color))
            {
                tween
                    .TweenProperty(laser.GetMaterial(), "albedo_color", new Color(keyFrame.Color), keyFrame.Duration)
                    .SetDelay(keyFrame.Time-lastTweenEnd);
            }
            
            if (lastTweenEndTime[keyFrame.LaserId] < keyFrame.Time + keyFrame.Duration)
            {
                lastTweenEndTime[keyFrame.LaserId] = keyFrame.Time + keyFrame.Duration;
            }
        }
        
        GD.Print("starting party mode");
        _audioListener.StartPartyMode();
        GD.Print("stopping alerts");
        AlertQueue.StopAlerts();
        
        VoiceMeeterService.MuteSpotify();


        var lastFrame = _showLoader.LaserKeyFrames.MaxBy(kf => kf.Time + kf.Duration);
        var duration = lastFrame.Time + lastFrame.Duration;
        GD.Print($"duration: {duration}");
        _ = Task.Run(async () =>
        {
            await Task.Delay((int)((duration+2) * 1000));
            VoiceMeeterService.UnmuteSpotify();
            IsPlaying = false;
            AlertQueue.ResumeAlerts();
            _audioListener.EndPartyMode();
            // dispose nodes and tweens
            
            foreach (var laser in _lasers)
            {
                laser.QueueFree();
            }
            
        });
    }
}