using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Godot;
using Temptica.TwitchBot.Bot.Alerts;
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
    protected bool AlertStarted { get; set; }
    
    public DateTime EventTime { get; set; }
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
            case RaidAlert:
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
    
    protected virtual Task ResetAlert()
    {
        IsFinished = false;
        ElapsedTime = 0;
        TtsStarted = false;
        return Task.CompletedTask;
    }

    public virtual Task ProcessAlert(double delta)
    {
        if (!AlertStarted)
        {
            
        }
        ElapsedTime += delta;
        if (ElapsedTime >= Duration)
        {
            IsFinished = true;
        }
        if(TimeTillTTS > 0 && ElapsedTime >= TimeTillTTS && !TtsStarted)
        {
            TtsStarted = true;
            DisplayServer.TtsSpeak(TTSMessage,Voices.Hazel);
        }
        return Task.CompletedTask;
    }
    
    public virtual Task StopAlert()
    {
        IsFinished = true;
        DisplayServer.TtsStop();
        return Task.CompletedTask;
    }

}