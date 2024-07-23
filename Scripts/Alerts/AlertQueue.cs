using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.Labels;
using Temptic404Overlay.Scripts.Models;
using Temptica.TwitchBot.Bot.Alerts;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Alerts;

public partial class AlertQueue : Node
{
    //signleton
    private static AlertQueue _instance;
    public static AlertQueue Instance => _instance ??= new AlertQueue();
    public string LastGifter { get; private set; } = "";
    public int GiftCount { get; private set; }
    public bool Paused { get; set; }

    private static readonly Queue<Alert> _queue = new();
    private Alert _currentAlert;
    private readonly List<Alert> _history = [];
    private static readonly List<Alert> AlertsToIgnore = [];
    
    private AlertQueue()
    {
    }
    
    public void SkipAllFollows()
    {
        //put all follows from queue to history
        var followsToRemove = new List<Alert>();
        foreach (var alert in _queue.OfType<FollowAlert>())
        {
            AlertsToIgnore.Add(alert);
        }
    }
    
    

    public static void AddAlert(OverlayAlert alert)
    {
        GD.Print($"Alert received: {alert.Type.ToString()}, {alert.User}, {alert.Message}, {alert.Amount}");
        switch (alert.Type)
        {
            //if alert is cheer, set last cheer label
            case AlertType.Bit:
                LastCheerLabel.OnAlert(null, alert);
                if (alert.Amount > 100)
                {
                    _queue.Enqueue(new BitsAlert(alert.User, alert.Amount, alert.Message,alert.Message ));
                    return; 
                }
                
                _queue.Enqueue(new BitsAlert(alert.User, alert.Amount, alert.Message));
                return;
            //if alert is sub, set last sub label
            case AlertType.Sub or AlertType.Resub:
                LastSubLabel.OnAlert(null, alert);
                _queue.Enqueue(new SubAlert(alert.User, alert.Message));
                return;
            //if alert is raid, set raid label
            case AlertType.Raid:
                RaidLabel.OnAlert(null, alert);
                _queue.Enqueue(new RaidAlert(alert.User, alert.Amount));
                return;
        }
    }

    public override void _Process(double delta)
    {
        CheckQueue(delta);
    }

    private void CheckQueue(double delta)
    {
        if (_queue.Count == 0 && _currentAlert == null) return; //nothing awaiting and nothing playing
        if(_currentAlert == null) //at least one alert awaiting and there is none playing
        {
            _currentAlert = _queue.Dequeue();
            if(AlertsToIgnore.Remove(_currentAlert)) //if alert should be ignored, remove will give true
            {
                return;
            }
        }
        _currentAlert.ProcessAlert(delta);
        if (!_currentAlert.IsFinished)//is still playing so just ignore
        {                
            return;
        }
        //stopped playing
        _history.Add(_currentAlert);
        _currentAlert = null; //make current alert null
        //if no alerts remaining in queue, start playing music
        if (_queue.Count == 0)
        {
            //await _spotifyService.Resume();
        }
    }
    
    public bool CheckLastFollowersCount()
    {
        //get count of follow alerts in queue and history in the past 30 seconds. return true if more than 5
        var count = _queue.Count(alert => alert is FollowAlert && alert.EventTime > DateTime.UtcNow.AddSeconds(-15));
        count += _history.Count(alert => alert is FollowAlert && alert.EventTime > DateTime.UtcNow.AddSeconds(-15));
        if (count <= 5) return false;
        return true;
    }
}