using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Godot;
using Temptic404Overlay.Scripts.Labels;
using Temptic404Overlay.Scripts.Models;
using Temptic404Overlay.Scripts.Services;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Alerts;

public abstract class Alert
{
    public AlertType Event { get; protected set; }
    public string User { get; protected set; }
    public string Message { get; protected set; }
    protected string TTSMessage { get; set; }        
    public bool IsFinished { get; protected set; }
    protected int Duration { get; set; }
    protected int TimeTillTTS { get; set; }
    private double ElapsedTime { get; set; }
    private bool TtsStarted { get; set; }
    private bool AlertStarted { get; set; }
    
    public DateTime EventTime { get; set; }
    
    protected TextToSpeechService TextToSpeechService => TextToSpeechService.Instance;

    public virtual bool StopMusic { get; internal init; } = true;

    protected Alert(string username)
    {
        EventTime = DateTime.UtcNow;
        User = username;
        IsFinished = false;
        Message = string.Empty;
        TTSMessage = string.Empty;
        Duration = 13000;
        TimeTillTTS = 0;
        ElapsedTime = 0d;
        switch (this)
        {
            case RaidAlert.RaidAlert:
            {
                Event = AlertType.Raid;
                break;
            }
            case ReSubAlert:
            case SubAlert:
            {
                Event = AlertType.Sub;
                break;
            }
            case BitsAlert:
            {
                Event = AlertType.Bit;
                break;
            }
            case FollowAlert:
            {
                Event = AlertType.Follow;
                break;
            }
            case GiftedAlert:
            {
                Event = AlertType.Gift;
                break;
            }
            default:
                Event = 0;
                break;
        }
    }
    
    protected virtual void ResetAlert()
    {
        IsFinished = false;
        ElapsedTime = 0;
        TtsStarted = false;
    }

    public void ProcessAlert(double delta)
    {
        if (!AlertStarted)
        {
            StartAlert();
            AlertStarted = true;
        }
        
        ElapsedTime += delta;
        
        if (ElapsedTime >= Duration && !TextToSpeechService.IsSpeaking && !TextToSpeechService.IsPaused)
        {
            IsFinished = true;
            StopAlert();
            return;
        }
        
        if(TimeTillTTS > 0 && ElapsedTime >= TimeTillTTS && !TtsStarted)
        {
            GD.Print("Starting TTS");
            TtsStarted = true;
            TextToSpeechService.Speak(TTSMessage);
        }

        UpdateAlert(delta);
    }

    protected virtual void UpdateAlert(double delta)
    {
        
    }
    public virtual void StopAlert()
    {
        IsFinished = true;
        TextToSpeechService.Stop();
        AlertLabels.HideAlert();
    }

    protected virtual void StartAlert()
    {
        switch (Event)
        {
            case AlertType.Follow:
                AudioPlayer.PlayAudio(AudioEffects.Follow);
                break;
            case AlertType.Gift:
            case AlertType.Resub:
            case AlertType.Sub:
                AudioPlayer.PlayAudio(AudioEffects.Subscription);
                break;
            case AlertType.Bit:
                AudioPlayer.PlayAudio(AudioEffects.Bits);
                break;
            case AlertType.Raid:
                AudioPlayer.PlayAudio(AudioEffects.Raid);
                break;
        }
        AlertLabels.ShowAlert(this);
    }

}