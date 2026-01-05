namespace Temptica.Overlay.Scripts.Alerts;

public class DonationAlert : Alert
{
    public DonationAlert(string username, string message) : base(username)
    {
        Duration = 20;
        TimeTillTTS = 19;
        TTSMessage = message;
        Message = message;
    }
}