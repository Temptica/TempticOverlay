using System.Threading.Tasks;
using Temptic404Overlay.Scripts.Alerts;

namespace Temptica.TwitchBot.Bot.Alerts
{
    internal class FollowAlert: Alert
    {
        public FollowAlert(string username) : base(username) { 
            Duration = 2000;
            TimeTillTTS = 0;
            
        }

        public override async Task ProcessAlert(double delta)
        {
            await base.ProcessAlert(delta);
        }
    }
}
