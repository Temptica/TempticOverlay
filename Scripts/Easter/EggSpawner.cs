using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Models;
using Temptica.Overlay.Scripts.Services;
using TwitcherSharp;
using TwitcherSharp.Chat;
using TwitcherSharp.EventSub.Generated.ChannelChatMessage;

namespace Temptica.Overlay.Scripts.Easter;

public partial class EggSpawner : Node3D
{
    private const int SpawnInterval = 10; //Minutes
    private DateTime _nextSpawnTime = DateTime.MinValue;
    private bool _enabled;

    private static KaniService EggService => KaniService.Instance;

    private readonly List<Egg> _eggs = [];

    public int EggCount { get; private set; }

    public override void _Ready()
    {
         _enabled = DateTime.Now.Month == 4; //Only during the month of Easter so April
        _nextSpawnTime = DateTime.Now.AddSeconds(3);
    }

    public override void _Process(double delta)
    {
        if (!_enabled || _nextSpawnTime >= DateTime.Now) return;

        //spawn logic
        _nextSpawnTime = DateTime.Now.AddMinutes(SpawnInterval);
        _ = SpawnEgg();
    }

    public async Task SpawnEgg()
    {
        var eggs = await EggService.GetAllEggNames();
        EggCount = eggs.Count;
        var eggName = eggs[new Random().Next(0, eggs.Count)];
        var dbEgg = await EggService.GetEggData(eggName);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        var egg = Egg.Create(this, dbEgg);
        egg.SetGlobalPosition(new Vector3((float)new Random().NextDouble() * 14 + 1,
            (float)new Random().NextDouble() * 7 + 1, -3));
        _eggs.Add(egg);
    }

    public async Task<bool> IsEggHit(Vector2 position, OverlayClickModel clickModel)
    {
        if (!_enabled)
        {
            return false;
        }
        
        var egg = _eggs.FirstOrDefault(e => e.CheckHit(position));

        if (egg == null) return false;

        var result = await EggService.CollectEgg(egg.Name, clickModel.UserId, clickModel.Username);

        if (!result) return false;

        var collection = await EggService.GetCollectedEggs(clickModel.UserId);
        var collectedEggsCount = collection.Sum(c => c.Amount ?? 0);
        var distinctCount = collection.Count;

        await TwitchChat.Instance.SendMessage(
            $"{clickModel.Username} collected {egg.Name} (by {egg.EggData.UserName}). " +
            $"They collected a total of {collectedEggsCount} eggs ({distinctCount}/{EggCount})!");

        _eggs.Remove(egg);
        egg.Remove();

        return false;
    }
}