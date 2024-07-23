using System.Threading.Tasks;
using Temptic404Overlay.Scripts.Alerts;

namespace Temptica.TwitchBot.Bot.Alerts
{
    internal class RaidAlert : Alert
    {
        public RaidAlert(string name, int count): base(name) 
        {
            Message = $"{name} and {count} viewers raided us!";
            Duration = 33000;
        }

        public override async Task ProcessAlert(double delta)
        {
            await base.ProcessAlert(delta);
        }
    }
}
