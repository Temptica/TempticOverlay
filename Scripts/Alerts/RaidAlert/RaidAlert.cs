using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Godot;

namespace Temptic404Overlay.Scripts.Alerts.RaidAlert
{
    internal class RaidAlert : Alert
    {
        public int Count { get; private set; }
        
        
        public static EventHandler<int> StartPlanes;
        private Plinko _plinko;
        
        public RaidAlert(string name, int count): base(name) 
        {
            Message = $"{name} and {count} viewers raided us!";
            Duration = PlaneSpawner.RaidDuration;
            Count = count;
        }

        protected override void StartAlert()
        {
            StartPlanes?.Invoke(this, Count);
            base.StartAlert();
        }
    }
}
