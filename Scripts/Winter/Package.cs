using System;
using Godot;
using Temptica.Overlay.Enums;

namespace Temptica.Overlay.Scripts.Winter;

public partial class Package : Node3D
{
    private bool _isHit;

    public void IsHit(Vector2 position, string userName)
    {
        if (_isHit) return;
        var pos = new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Y);
        if (position.DistanceTo(pos) > 1.5f) return;

        var isBadPresent = new RandomNumberGenerator().RandiRange(0, 1) == 0;
        Hide();
        _isHit = true;

        if (isBadPresent)
        {
            OpenBadPresent(userName);
        }
        else
        {
            OpenGoodPresent(userName);
        }
        
        //Display
        

    }

    private void OpenGoodPresent(string userName)
    {
        // var values = Enum.GetValues<GoodPresentTypes>();
        // var type = values[new Random().Next(values.Length)];
        //
        // switch (type)
        // {
        //     case GoodPresentTypes.GiveawayTicket:
        //         //Give Ticket Via Api
        //         break;
        //     case GoodPresentTypes.Vip:
        //         //Give Vip Via Api
        //         break;
        //     case GoodPresentTypes.Sub:
        //         //Do Nothing
        //         break;
        //     case GoodPresentTypes.ChooseRaid:
        //         //DO nothing
        //         break;
        //     case GoodPresentTypes.GivePoints:
        //         //Give Points Via Api
        //         break;
        //     case GoodPresentTypes.Compliment:
        //         // Do nothing
        //         break;
        //     case GoodPresentTypes.SteamKey:
        //         // Do nothing
        //         break;
        //     case GoodPresentTypes.PixelArt:
        //         // Do nothing
        //         
        //         break;
        //     case GoodPresentTypes.SmallSong:
        //         // Do nothing
        //         
        //         break;
        //     case GoodPresentTypes.TimeOutSomebodyElse:
        //         // Do nothing
        //         break;
        //     case GoodPresentTypes.Explode:
        //         // Explode otter
        //         for (var i = 0; i < 7; i++)
        //         {
        //             Otter..
        //             OtterSignalRListener.Pixelate.Invoke(this, null!);
        //         }
        //         break;
        //     case GoodPresentTypes.SpawnFishes:
        //         // StartSpawningfishes
        //         SpawnFishListener.SpawnFish.Invoke(this, 20);
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
    }

    private void OpenBadPresent(string userName)
    {
        var values = Enum.GetValues<BadPresentTypes>();
        var type = values[new Random().Next(values.Length)];

        switch (type)
        {
            case BadPresentTypes.GetTimedOut:
                //APi
                break;
            case BadPresentTypes.Nothing:
                //Do nothing
                break;
            case BadPresentTypes.OnePoint:
                //Api
                break;
            case BadPresentTypes.LooseAllPoints:
                //Api
                break;
            case BadPresentTypes.WallOfShame:
                //get profile picture
                break;
            case BadPresentTypes.NaughtyCorner:
                //get profile picture
                break;
            case BadPresentTypes.LaunchPerson:
                //Get profile picture
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}