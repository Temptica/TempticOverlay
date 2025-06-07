using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using Temptica.Overlay.scenes;

namespace Temptica.Overlay.Scripts.SignalR.Listeners.GameListeners;

public class StartFishBattle : ISignalRListener
{
    public StartFishBattle(HubConnection connection)
    {
        connection.On("StartFishBattle", ()  =>
        {
            FishDuel.ShouldStartBattle = true;
            GD.Print("Recieved StartFishBattle");
        });
        
    }
    
}