using System;
using Temptica.Overlay.Scripts.Extensions;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.Scripts.Alerts
{
    internal sealed class BitsAlert : Alert
    {
        public int BitAmount { get; private set; }    
        public static EventHandler<string> StartShow;

        public BitsAlert(string username, int bitAmount, string tts, string messages = ""):base(username)
        {
            BitAmount = bitAmount;
            Event = AlertType.Bit;
            TTSMessage = tts.CleanEmoteName();
            Message = messages;
            Duration = 5;
            TimeTillTTS = 2;
            if (BitAmount == 500)
            {
                TimeTillTTS = 0;
                StopMusic = false;
            }
        }
        public BitsAlert(string username, int bitAmount, string messages) : base(username)
        {
            BitAmount = bitAmount;
            Event = AlertType.Bit;
            Message = messages;
            Duration = 4;
            TimeTillTTS = 2;
            if (BitAmount == 500)
            {
                TimeTillTTS = 1;
                StopMusic = false;
            }
        }

        protected override void StartAlert()
        {
            //base.StartAlert();

            if (BitAmount < 100)
            {
                switch (BitAmount)
                {
                    case 50:
                        AudioPlayer.PlayAudio(AudioEffects.Flash);
                        break;
                    default:
                        base.StartAlert();
                        break;

                }
            }
            else
            {
                if (BitAmount == 500)
                {
                    //StartShow?.Invoke(this, "Up&Down");
                    //TTSMessage = "";
                    //return;
                }
                base.StartAlert(); 
            }
        }
        
    }
}
