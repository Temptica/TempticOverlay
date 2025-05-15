using System.Threading.Tasks;

namespace Temptica.Overlay.Scripts.Alerts
{
    internal class FollowAlert: Alert
    {
        public FollowAlert(string username) : base(username) { 
            Duration = 3;
            TimeTillTTS = 0;
        }
    }
}
