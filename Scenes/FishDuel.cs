using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Temptica.TwitchBot.Shared.enums;

namespace Temptica.Overlay.scenes;

public partial class FishDuel : Node3D
{
    [Export] private MeshInstance3D _leftMeshInstance;
    [Export] private MeshInstance3D _rightMeshInstance;
    [Export] private MeshInstance3D _centerMeshInstance;
    [Export] private StandardMaterial3D _redFishMaterial;
    [Export] private StandardMaterial3D _greenFishMaterial;
    [Export] private StandardMaterial3D _pinkFishMaterial;
    [Export] private StandardMaterial3D _purpleFishMaterial;
    
    private PackedScene _fishScene;
    
    public static bool ShouldStartBattle { get; set; }

    private bool BattleInProgress { get; set; }

    private DateTime BattleStartTime { get; set; }
    private bool _startingBattle;
    private bool _shouldStartNextBattle;
    private bool _shouldEndBattle;

    private const int BattleDuration = 30;

    private FishColor[] _battleOneColors = new FishColor[2];
    private FishColor[] _battleTwoColors = new FishColor[2];
    private FishColor[] _battleThreeColors = new FishColor[2];

    private FishColor[] _currentBattle = new FishColor[2];
    public override void _Ready()
    {
        _fishScene = GD.Load<PackedScene>("res://Templates/battle_fish.tscn");
    }

    public override void _Process(double delta)
    {
        if (_shouldEndBattle)
        {
            _shouldEndBattle = false;
            ShouldStartBattle = false;
            BattleInProgress = false;
            var fishes = GetChildren().Where(f=>f is Scripts.Fishes.BattleFish).Cast<Scripts.Fishes.BattleFish>().ToList();
            foreach (var fish in fishes)
            { 
                fish.QueueFree();
            }
            _leftMeshInstance.Hide();
            _rightMeshInstance.Hide();
            _centerMeshInstance.Hide();
        }
        if (ShouldStartBattle)
        {
            ShouldStartBattle = false;
            StartBattles(); 
        }
        
        if (_shouldStartNextBattle)
        {
            _shouldStartNextBattle = false;
            StartBattle();
            return;
        }
        
        if (!BattleInProgress && !_shouldStartNextBattle) return;
        
        if (BattleInProgress && !_startingBattle)
        { 
            var fishes = GetChildren().Where(f=>f is Scripts.Fishes.BattleFish).Cast<Scripts.Fishes.BattleFish>().ToList();
            var percentage = FishPercentage();
            
            //scale  _leftMeshInstance (4 is the max scale)
            var mesh = (BoxMesh)_leftMeshInstance.Mesh;
            var size = percentage * 4;
            mesh.Size = new Vector3(size, mesh.Size.Y, mesh.Size.Z);
            _leftMeshInstance.Mesh = mesh;
            
            _leftMeshInstance.Position = new Vector3(8 - (percentage * 4 - 4) / 2, _leftMeshInstance.Position.Y, _leftMeshInstance.Position.Z);
            
            if (DateTime.Now - BattleStartTime < TimeSpan.FromSeconds(BattleDuration))
            {
                return;
            }
            
            var firstColorCount = fishes.Count(f => f.Color == _currentBattle[0]);
            var secondColorCount = fishes.Count(f => f.Color == _currentBattle[1]);
            
            if(firstColorCount == secondColorCount )
            {
                return;
            }
            
            if (firstColorCount > secondColorCount)
            {
                
                if(_battleThreeColors[0] == FishColor.None)
                {
                    _battleThreeColors[0] = _currentBattle[0];
                }
                else if(_battleThreeColors[1] == FishColor.None)
                {
                    _battleThreeColors[1] = _currentBattle[0];
                }
                else
                {
                    EndBattle(_currentBattle[0]);
                    _rightMeshInstance.Hide();
                    _centerMeshInstance.Hide();
                    return;
                }
                _rightMeshInstance.Hide();
                _centerMeshInstance.Hide();
                EndBattle();
            }
            else if (firstColorCount < secondColorCount)
            {
                GD.Print("Second color wins!");
                
                if(_battleThreeColors[0] == FishColor.None)
                {
                    _battleThreeColors[0] = _currentBattle[1];
                }
                else if(_battleThreeColors[1] == FishColor.None)
                {
                    _battleThreeColors[1] = _currentBattle[1];
                }
                else
                {
                    GD.Print($"{_currentBattle[1]} Has won this tournament!");
                    EndBattle(_currentBattle[1]);
                    _leftMeshInstance.Hide();
                    _centerMeshInstance.Hide();
                    return;
                }
                _leftMeshInstance.Hide();
                _centerMeshInstance.Hide();
                EndBattle();
            }
            else
            {
                if(_battleThreeColors[0] == FishColor.None)
                {
                    _battleThreeColors = _battleTwoColors;
                }
                else if(_battleThreeColors[1] == FishColor.None)
                {
                    GD.Print($"{_battleThreeColors[0]} Has won this tournament!");
                    EndBattle(_battleThreeColors[0]);
                    return;
                }
                else
                {
                    GD.Print($"{_currentBattle[1]} Has won this tournament!");
                    EndBattle(_currentBattle[1]);
                    return;
                }
                EndBattle();
            }
        }
    }
    
    private void EndBattle(FishColor winnerFish = FishColor.None)
    {
        BattleInProgress = false;
        ShouldStartBattle = false;
        var fishes = GetChildren().Where(f=>f is Scripts.Fishes.BattleFish).Cast<Scripts.Fishes.BattleFish>().ToList();
        
        foreach (var fish in fishes.Where(fish => fish.Color != _battleThreeColors[0] && fish.Color != _battleThreeColors[1]))
        {
            fish.QueueFree();
        }
        
        //show the winning color on overlay
        _ = Task.Run(async () =>
        {
            await Task.Delay(7000);
            if (winnerFish == FishColor.None)
            {
                _shouldStartNextBattle = true;
                GD.Print("Battle ended, starting next one");
            }
            else
            {
                _shouldEndBattle = true;
                Scripts.Overlay.SignalRService.DuelWinnerColor(winnerFish);
                GD.Print("Battle ended, winner is: " + winnerFish);
            }
        });
    }

    private void StartBattles()
    {
        _battleOneColors = [FishColor.None, FishColor.None];
        _battleTwoColors = [FishColor.None, FishColor.None];
        _battleThreeColors = [FishColor.None, FishColor.None];
        
        var rng = new Random();
        var colors = new [] {FishColor.Red, FishColor.Green, FishColor.Pink, FishColor.Purple};
        var firstColor = colors[rng.Next(0,colors.Length)];
        colors = colors.Where(c => c != firstColor).ToArray();
        var secondColor = colors[rng.Next(0,colors.Length)];
        colors = colors.Where(c => c != secondColor).ToArray();
        
        _battleOneColors[0] = firstColor;
        _battleOneColors[1] = secondColor;
        _battleTwoColors[0] = colors[0];
        _battleTwoColors[1] = colors[1];
        
        _battleThreeColors = [FishColor.Green, FishColor.Red];
        
        _currentBattle = _battleOneColors;
        
        _ = Task.Run(async () =>
        {
            await Task.Delay(60000);
            //call signalr to start the predictions
            _shouldStartNextBattle = true;
        });
    }
    private void StartBattle()
    {
        //remove all existing fishes
        _shouldStartNextBattle = false;
        _startingBattle = true;
        GD.Print("Starting battle");
        foreach (var fish in GetChildren().Where(f=>f is Scripts.Fishes.BattleFish).Cast<Scripts.Fishes.BattleFish>().ToList())
        {
            fish.QueueFree();
        }
        
        if (_battleThreeColors[0] is FishColor.None)
        {
            _currentBattle = _battleOneColors;
            GD.Print("Starting battle one.");
            
        }
        else if(_battleThreeColors[1] is FishColor.None)
        {
            _currentBattle = _battleTwoColors;
            GD.Print("Starting battle two.");
        }
        else
        {
            _currentBattle = _battleThreeColors;
            GD.Print("Starting final battle.");
        }
        
        for (int i = 0; i < 50; i++)
        {
            SpawnFish(_currentBattle[0]);
            SpawnFish(_currentBattle[1]);
        }
        _leftMeshInstance.Mesh.SurfaceSetMaterial(0, GetMaterial(_currentBattle[0]));
        _rightMeshInstance.Mesh.SurfaceSetMaterial(0, GetMaterial(_currentBattle[1]));
        _leftMeshInstance.Show();
        _rightMeshInstance.Show();
        _centerMeshInstance.Show();
        
        BattleStartTime = DateTime.Now;
        GD.Print($"Battle started at {BattleStartTime}");
        _startingBattle = false;
        BattleInProgress = true;
    }
    private void SpawnFish(FishColor color)
    {
        var fish = (Scripts.Fishes.BattleFish)_fishScene.Instantiate();
        fish.SetFishColor(color);
        AddChild(fish);
        fish.GlobalPosition = new Vector3((float)new Random().NextDouble() * 14 + 1, (float)new Random().NextDouble() * 7 + 1,0);
    }
    
    private StandardMaterial3D GetMaterial(FishColor color)
    {
        return color switch
        {
            FishColor.Red => _redFishMaterial,
            FishColor.Green => _greenFishMaterial,
            FishColor.Pink => _pinkFishMaterial,
            FishColor.Purple => _purpleFishMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    } 
    
    private float FishPercentage()
    {
        var fishes = GetChildren().Where(f=>f is Scripts.Fishes.BattleFish).Cast<Scripts.Fishes.BattleFish>().ToList();
        var colorCount = fishes.Count(f => f.Color == _currentBattle[0]);
        return colorCount / (float)fishes.Count;
    }
}