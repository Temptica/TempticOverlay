using System.Threading.Tasks;
using Temptic404Overlay.Scripts.Alerts;

namespace Temptica.TwitchBot.Bot.Alerts
{
    internal class SubAlert : Alert
    {
        public SubAlert(string username) : base(username) { }
       /* public SubAlert(string username, SubscriptionPlan subscriptionPlan) : this(username)
        {
            var tier = subscriptionPlan switch
            {
                SubscriptionPlan.NotSet => string.Empty,
                SubscriptionPlan.Prime => "with a prime sub",
                SubscriptionPlan.Tier1 => "with a tier 1 sub",
                SubscriptionPlan.Tier2 => "with a tier 2 sub",
                SubscriptionPlan.Tier3 => "with a tier 3 sub",
                _ => string.Empty,
            };
            TTSMessage = $"{username} just subscribed {tier}.";
            Duration = 9000;
            TimeTillTTS = 6000;
        }*/
        public SubAlert(string username, string tts) : this(username)
        {
            TTSMessage = tts;
        }

        public override async Task ProcessAlert(double delta)
        {
            await base.ProcessAlert(delta);
        }
    }
}
