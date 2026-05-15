using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Temptica.Overlay.Scripts.Fishes;
using Temptica.Overlay.Scripts.Labels;
using Temptica.Overlay.Scripts.Spotify;
using TwitcherSharp.Chat;
using TwitcherSharp.EventSub;
using TwitcherSharp.EventSub.Generated.ChannelCheer;
using TwitcherSharp.EventSub.Generated.ChannelFollow;
using TwitcherSharp.EventSub.Generated.ChannelRaid;
using TwitcherSharp.EventSub.Generated.ChannelSubscribe;
using TwitcherSharp.EventSub.Generated.ChannelSubscriptionMessage;
using TwitcherSharp.Reward;

namespace Temptica.Overlay.Scripts.Alerts;

public partial class AlertQueue : Node
{
    //signleton
    public static AlertQueue Instance { get; private set; }
    public string LastGifter { get; private set; } = "";
    public int GiftCount { get; private set; }
    public static bool Paused { get; private set; }

    private static readonly Queue<Alert> _queue = new();
    private static Alert _currentAlert;
    private static readonly List<Alert> _history = [];
    private static readonly List<Alert> AlertsToIgnore = [];
    private double _timeSinceLast = TimeBetweenAlerts;

    private const int TimeBetweenAlerts = 2;

    public override void _Ready()
    {
        Instance = this;
        
        TwitchRedeemListener.FromObject(GetNode<GodotObject>("TtsRedeemListener")).Redeemed += OnTts;
        
        TwitchEventListener<TwitchChannelFollowEvent>.FromObject(GetNode("FollowEventListener")).Received += OnFollow;
        
        TwitchEventListener<TwitchChannelSubscribeEvent>
            .FromObject(GetNode<GodotObject>("SubEventListener")).Received += OnSub;

        TwitchEventListener<TwitchChannelSubscriptionMessageEvent>.FromObject(
            GetNode<GodotObject>("ReSubEventListener"))
            .Received += OnReSub;
        
        TwitchEventListener<TwitchChannelCheerEvent>.FromObject(
            GetNode<GodotObject>("CheerEventListener"))
            .Received += OnCheer;
        
        TwitchEventListener<TwitchChannelRaidEvent>.FromObject(
            GetNode<GodotObject>("RaidEventListener"))
            .Received += OnRaid;
        
        TwitchCommand.FromObject(GetNode<GodotObject>("SkipSpeechTwitchCommand")).CommandReceived += (username, _, args) =>
        {
            var msg = string.Join(" ", args);
            _queue.Enqueue(new TtsAlert($"{username} says: {msg}"));
        };

        GetNode<GodotObject>("SkipSpeechTwitchCommand").Connect("command_received",
            Callable.From<string, GodotObject, Array<string>>(SkipSpeech));
        GetNode<GodotObject>("StopAllAlertsTwitchCommand").Connect("command_received",
            Callable.From<string, GodotObject, Array<string>>(SkipAll));
    }

    public static void SkipAllFollows() 
    {
        AlertsToIgnore.AddRange(_queue.OfType<FollowAlert>());
    }

    private static void OnFollow(TwitchChannelFollowEvent data)
    {
        _queue.Enqueue(new FollowAlert("A new otter"));
        FishSpawner.SpawnFishes(1);
    }

    private static void OnSub(TwitchChannelSubscribeEvent data)
    {
        var userName = data.UserName;
        var tier = int.Parse(data.Tier)/1000;
        var tts = $"{userName} just subscribed with a tier {tier} sub";
        LastSubLabel.OnAlert(userName, tier);
        FishSpawner.SpawnFishes(tier);
        _queue.Enqueue(new SubAlert(userName, tts));
    }

    private static void OnReSub(TwitchChannelSubscriptionMessageEvent data)
    {
        var userName = data.UserName;
        var message = data.Message.Text;
        var tier = int.Parse(data.Tier);
        var cul = data.CumulativeMonths;
        var streak = data.StreakMonths;
        var tts = $"{userName} just resubscribed with a tier {tier} sub for a total of {cul} months ";
        if (streak != 0)
        {
            tts += $" with a streak of {streak} ";
        }

        if (!string.IsNullOrEmpty(message))
        {
            tts += $". {message}";
        }

        LastSubLabel.OnAlert(userName, tier);
        FishSpawner.SpawnFishes(tier);
        _queue.Enqueue(new ReSubAlert(userName, message, tts));
    }

    private static void OnCheer(TwitchChannelCheerEvent data)
    {
        var userName = data.UserName;
        var bits = data.Bits;
        var message = data.Message;
        LastCheerLabel.OnAlert(userName, bits);
        if (bits < 100)
        {
            _queue.Enqueue(new BitsAlert(userName, bits, message));
            return;
        }

        var tts = $"{userName} just threw {bits} bits. {message}";
        _queue.Enqueue(new BitsAlert(userName, bits, tts, message));
        FishSpawner.SpawnFishes((int)Math.Floor(bits / 100d));
    }

    private static void OnRaid(TwitchChannelRaidEvent data)
    {
        var raider = data.FromBroadcasterUserName;
        var viewers = data.Viewers;
        RaidLabel.OnAlert(raider, viewers);
        FishSpawner.SpawnFishes(viewers);
        _queue.Enqueue(new RaidAlert.RaidAlert(raider, viewers));
    }
    private static void OnTts(TwitchRedemption redemption)
    {
        _queue.Enqueue(new TtsAlert($"{redemption.User.DisplayName} says: {redemption.UserInput}"));
    }

    public override void _Process(double delta)
    {
        CheckQueue(delta);
    }

    private void CheckQueue(double delta)
    {
        if ((_queue.Count == 0 && _currentAlert == null) || Paused) return; //nothing awaiting and nothing playing

        if (_currentAlert == null) //at least one alert awaiting and there is none playing
        {
            if (_timeSinceLast < TimeBetweenAlerts)
            {
                _timeSinceLast += delta;
                return;
            }

            _timeSinceLast = 0;

            _currentAlert = _queue.Dequeue();
            if (_currentAlert.StopMusic)
            {
                _ = SpotifyService.Pause();
            }

            if (AlertsToIgnore.Remove(_currentAlert)) //if alert should be ignored, remove will give true
            {
                return;
            }
        }

        _currentAlert.ProcessAlert(delta);
        if (!_currentAlert.IsFinished) //is still playing so just ignore
        {
            return;
        }

        //stopped playing
        _history.Add(_currentAlert);
        _currentAlert = null; //make current alert null

        //if no alerts remaining in queue, start playing music
        if (_queue.Count == 0)
        {
            _ = SpotifyService.Resume();
            _timeSinceLast = TimeBetweenAlerts;
        }

        _timeSinceLast += delta;
    }

    private static bool CheckLastFollowersCount()
    {
        //get count of follow alerts in queue and history in the past 30 seconds. return true if more than 5
        var count = _queue.Count(alert => alert is FollowAlert && alert.EventTime > DateTime.UtcNow.AddSeconds(-15));
        count += _history.Count(alert => alert is FollowAlert && alert.EventTime > DateTime.UtcNow.AddSeconds(-15));

        return count > 5;
    }

    public static void ReplayLastAlerts(int count = 1)
    {
        _history
            .TakeLast(count)
            .ToList()
            .ForEach(a => _queue.Enqueue(a));
    }

    public static void ResumeAlerts()
    {
        Paused = false;
    }

    public static void StopAlerts()
    {
        Paused = true;
        _currentAlert.StopAlert();
    }

    public static void PauseNextAlert()
    {
        Paused = true;
    }

    public static void SkipSpeech(string userName, GodotObject info, Array<string> args)
    {
        _currentAlert.SkipSpeech();
    }

    public static void SkipAll(string s, GodotObject godotObject, Array<string> arg3)
    {
        _queue.Clear();
    }
}