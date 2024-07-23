using System.Threading.Tasks;

namespace Temptic404Overlay.Scripts.Alerts
{
    internal class GiftedAlert : Alert
    {
        public string Gifter { get; set; }
        public GiftedAlert(string gifter,string reciever) : base(reciever) {
            Gifter = gifter;
            Message = TTSMessage = $"{gifter} has gifted a sub to {reciever} to the community!";
            Duration = 9000;
            TimeTillTTS = 6000;
        }

        public override async Task ProcessAlert(double delta)
        {
            await base.ProcessAlert(delta);
        }
    }
}
