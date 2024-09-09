namespace Temptic404Overlay.Scripts.Alerts
{
    internal class ReSubAlert : SubAlert
    {
        public ReSubAlert(string username, string msg, string tts) : base(username)
        {
            Message = msg;
            TTSMessage = tts;
        }
    }
}

