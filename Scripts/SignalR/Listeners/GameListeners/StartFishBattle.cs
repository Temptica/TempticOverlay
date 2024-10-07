using System;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;

namespace Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;

public class StartFishBattle : ISignalRListener
{
    public StartFishBattle(HubConnection connection)
    {
        connection.On("StartFishBattle", ()  =>
        {
            Scenes.FishDuel.ShouldStartBattle = true;
            GD.Print("Recieved StartFishBattle");
        });
        
    }
    
}