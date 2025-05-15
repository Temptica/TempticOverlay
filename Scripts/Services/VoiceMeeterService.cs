using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Godot;
using Voicemeeter;

namespace Temptica.Overlay.Scripts.Services;
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
            GD.Print("Login in ...");
            _disposable = await Remote.Initialize(Type);

            if (_disposable is null)
            {
                GD.Print("Failed to login to VoiceMeeter");
                return;
            }
            
            GD.Print("Staring");
            Remote.Start(Type);
            GD.Print("started");
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            throw;
        }
    }
    
    public static void MuteSpotify()
    {
        try
        {
            Remote.SetParameter("Strip[0].Mute", 1);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            throw;
        }
    }
    
    public static void UnmuteSpotify()
    {
        try
        {
            Remote.SetParameter("Strip[0].Mute", 0);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            throw;
        }
    }

    public static void Logout()
    {
        _disposable.Dispose();
    }
    
}