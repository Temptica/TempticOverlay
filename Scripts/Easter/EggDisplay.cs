using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.Overlay.Scripts.Models;
using Temptica.Overlay.Scripts.Services;
using TwitcherSharp;
using TwitcherSharp.Api.Generated.Users;
using TwitcherSharp.Chat;
using TwitcherSharp.Extensions;

namespace Temptica.Overlay.Scripts.Easter;

public partial class EggDisplay : Node3D
{
    private bool _isShowingEggs;
    private Queue<EggMetaData> _eggsToSpawn = [];
    private Timer _eggSpawnTimer;
    private const float MaxX = 16f;

    public override void _Ready()
    {
        ProcessMode = DateTime.Now.Month is 4 ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;

        if (ProcessMode == ProcessModeEnum.Disabled) return;
        
        /*
        var listener = this.GetTwitcherNode<TwitchCommand>("TwitchCommand");

        listener.CommandReceived += async (username, _, _) =>
        {
            if (_isShowingEggs) return;
            var user = await TwitchService.Instance.GetUser(username);

            _isShowingEggs = true;
            await ShowEggs(user);
        };*/
    }

    private async Task ShowEggs(TwitchUser user)
    {
        var userEggs = await KaniService.Instance.GetCollectedEggs(user.Id);
        var collectedEggsCount = userEggs.Sum(c => c.Amount ?? 0);
        var distinctCount = userEggs.Select(c => c.EggName).Distinct().Count();

        await TwitchChat.Instance.SendMessage(
            $"{user.DisplayName} collected a total of {collectedEggsCount} eggs ({distinctCount})!");
        _eggsToSpawn =
            new Queue<EggMetaData>(await KaniService.Instance.GetEggsData(userEggs.Select(e => e.EggName).ToArray()));
        var timer = new Timer();
        timer.SetWaitTime(0.5d);

        timer.Timeout += SpawnEgg;
    }

    private void SpawnEgg()
    {
        if (!_eggsToSpawn.TryDequeue(out var eggData))
        {
            _isShowingEggs = false;
            _eggSpawnTimer.Stop();
            _eggSpawnTimer.QueueFree();
            _eggSpawnTimer = null;
            return;
        }

        var egg = DisplayEgg.Create(this, eggData);
        egg.SetGlobalPosition(new Vector3(new Random().NextSingle() * MaxX, 0, 0));
    }
}