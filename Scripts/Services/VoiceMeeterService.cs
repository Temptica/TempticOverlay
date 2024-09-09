using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Godot;
using Voicemeeter;

namespace Temptic404Overlay.Scripts.Services;
using VoiceMeeter;

public class VoiceMeeterService
{
    
    private const RunVoicemeeterParam Type = RunVoicemeeterParam.VoicemeeterPotato;
    private static IDisposable _disposable;
    private static bool _loggedIn = false;

    public static async Task LogIn()
    {
        try
        {
            _loggedIn = await Remote.Login(Type);

            if (!_loggedIn)
            {
                GD.Print("Failed to login to VoiceMeeter");
                return;
            }
        
            Remote.Start(Type);
            
            _disposable = await Remote.Initialize(Type);
            
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            throw;
        }
    }
    
    public static async Task MuteSpotify()
    {
        try
        {
            //Mute Spotify
            Remote.SetParameter("Strip[0].Mute", 1);
        }
        catch (Exception e)
        {
            //wait for the user to login to VoiceMeeter
            
            
            GD.PrintErr(e);
            throw;
        }
    }
    
    public static async Task UnmuteSpotify()
    {
        try
        {
            //Unmute Spotify
            Remote.SetParameter("Strip[0].Mute", 0);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            throw;
        }
    }

    public static async Task Logout()
    {
        _disposable.Dispose();
    }
    
}