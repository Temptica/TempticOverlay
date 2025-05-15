using System.Threading.Tasks;

namespace Temptica.Overlay.Scripts.Alerts
{
    internal class GiftedAlert : Alert
    {
        public string Gifter { get; set; }
        public GiftedAlert(string gifter,string reciever) : base(reciever) {
            Gifter = gifter;
            Message = TTSMessage = $"{gifter} has gifted a sub to {reciever} to the community!";
            Duration = 9;
            TimeTillTTS = 6;
        }
    }
}
