using System.Threading.Tasks;

namespace Temptic404Overlay.Scripts.Alerts
{
    internal class SubAlert : Alert
    {
        public SubAlert(string username) : base(username)
        {
            TTSMessage = $"{username} just subscribed.";
            Duration = 9;
            TimeTillTTS = 7;
        }
        public SubAlert(string username, string tts) : this(username)
        {
            TTSMessage = tts;
        }
    }
}
