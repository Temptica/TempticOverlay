using Temptica.Overlay.Scripts.Extensions;

namespace Temptica.Overlay.Scripts.Alerts
{
    internal class ReSubAlert : SubAlert
    {
        public ReSubAlert(string username, string msg, string tts) : base(username)
        {
            Message = msg;
            TTSMessage = tts.CleanEmoteName();
        }
    }
}

