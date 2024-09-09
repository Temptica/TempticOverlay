using System.Threading.Tasks;

namespace Temptic404Overlay.Scripts.Alerts
{
    internal class FollowAlert: Alert
    {
        public FollowAlert(string username) : base(username) { 
            Duration = 2;
            TimeTillTTS = 0;
            
        }
    }
}
