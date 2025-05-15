using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Fishes;
using Temptica.Overlay.Scripts.Labels;
using Temptica.Overlay.Scripts.Models;
using Temptica.Overlay.Scripts.Spotify;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.Alerts;

public partial class AlertQueue : Node
{
	//signleton
	private static AlertQueue _instance;
	public static AlertQueue Instance => _instance ??= new AlertQueue();
	public string LastGifter { get; private set; } = "";
	public int GiftCount { get; private set; }
	public static bool Paused { get; private set; }

	private static readonly Queue<Alert> _queue = new();
	private static Alert _currentAlert;
	private static readonly List<Alert> _history = [];
	private static readonly List<Alert> AlertsToIgnore = [];
	private double _timeSinceLast = TimeBetweenAlerts;

	private const int TimeBetweenAlerts = 2;
	
	private AlertQueue()
	{
	}
	
	public static void SkipAllFollows()
	{
		AlertsToIgnore.AddRange(_queue.OfType<FollowAlert>());
	}
	
	public static void AddAlert(OverlayAlert alert)
	{
		switch (alert.Type)
		{
			//if alert is cheer, set last cheer label
			case AlertType.Bit:
				LastCheerLabel.OnAlert(null, alert);
				FishSpawner.SpawnFishes((int)Math.Floor(alert.Amount/100d));
				if (alert.Amount >= 100)
				{
					_queue.Enqueue(new BitsAlert(alert.User, alert.Amount, alert.Message,alert.Message ));
					return; 
				}
				
				_queue.Enqueue(new BitsAlert(alert.User, alert.Amount, alert.Message));
				return;
			//if alert is sub, set last sub label
			case AlertType.Sub or AlertType.Resub:
				LastSubLabel.OnAlert(null, alert);
				FishSpawner.SpawnFishes(alert.Amount/1000);
				_queue.Enqueue(new SubAlert(alert.User, alert.Message));
				return;
			//if alert is raid, set raid label
			case AlertType.Raid:
				RaidLabel.OnAlert(null, alert);
				FishSpawner.SpawnFishes(alert.Amount);
				_queue.Enqueue(new RaidAlert.RaidAlert(alert.User, alert.Amount));
				return;
			case AlertType.Follow:
				//if alert is follow, set last follow label
				_queue.Enqueue(new FollowAlert(alert.User));
				FishSpawner.SpawnFishes(1);
				return;
		}
	}
	
	public static void AddAlert(Alert alert)
	{
		_queue.Enqueue(alert);
	}

	public override void _Process(double delta)
	{
		CheckQueue(delta);
	}

	private void CheckQueue(double delta)
	{
		if ((_queue.Count == 0 && _currentAlert == null) || Paused) return; //nothing awaiting and nothing playing
		
		if(_currentAlert == null) //at least one alert awaiting and there is none playing
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
			_ = SpotifyService.Resume();
			_timeSinceLast = TimeBetweenAlerts;
		}
		
		_timeSinceLast += delta;
	}

	private bool CheckLastFollowersCount()
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
			.ForEach(a=>_queue.Enqueue(a));
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

	public static void SkipSpeech()
	{
		_currentAlert.SkipSpeech();
	}

	public static void SkipAll()
	{
		_queue.Clear();
	}
}
