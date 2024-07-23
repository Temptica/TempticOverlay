using System.Threading.Tasks;
using Godot;
using Temptica.TwitchBot.Shared.enums;

namespace Temptic404Overlay.Scripts.Alerts
{
    internal class BitsAlert : Alert
    {
        public int BitAmount { get; private set; }    

        public BitsAlert(string username, int bitAmount, string tTS, string messages = ""):base(username)
        {
            BitAmount = bitAmount;
            Event = AlertType.Bit;
            TTSMessage = tTS;
            Message = messages;
            Duration = 4000;
            TimeTillTTS = 2000;
        }
        public BitsAlert(string username, int bitAmount, string messages) : base(username)
        {
            BitAmount = bitAmount;
            Event = AlertType.Bit;
            Message = messages;
            Duration = 4000;
            TimeTillTTS = 2000;
        }

        public override Task StopAlert()
        {
            return base.StopAlert();
        }
    }
}
