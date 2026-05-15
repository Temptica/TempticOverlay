using Godot;
using System.Threading.Tasks;
using TwitcherSharp.Extensions;
using Environment = Godot.Environment;

public partial class OverlayColorManager : Node
{
    public static OverlayColorManager Instance { get; private set; }

    [Signal]
    public delegate void OnStartPartyModeEventHandler();

    [Signal]
    public delegate void OnStopPartyModeEventHandler();
    
    [Signal]
    public delegate void SetGlowColorEventHandler(Color color);

    [Signal]
    public delegate void ResetGlowColorEventHandler();

    private Environment _environment;

    public bool PartyModeEnabled { get; private set; }
    public bool PartyModePaused { get; private set; }
    
    const double GlowIntensity = 0.75d;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        GetNode("PartyModeTwitchRedeemListener").ConnectRedeemed(StartPartyMode);
        GetNode("StartPartyModeTwitchCommand").ConnectCommandReceived(StartPartyMode);
        GetNode("StopPartyModeTwitchCommand").ConnectCommandReceived(StopPartyMode);
        _environment = GetNode<Camera3D>("%Camera").Environment;
    }

    public void StartPartyMode()
    {
        PartyModeEnabled = true;
        PartyModePaused = false;
        EmitSignalOnStartPartyMode();
    }

    public void PausePartyMode()
    {
        EmitSignalOnStopPartyMode();
        PartyModePaused = true;
    }

    public void ResumePartyMode()
    {
        if (PartyModeEnabled) EmitSignalOnStartPartyMode();
        PartyModePaused = false;
    }

    public void StopPartyMode()
    {
        PartyModeEnabled = false;
        EmitSignalOnStopPartyMode();
    }

    public async void PlayFlashBang()
    {
        if (PartyModeEnabled)
        {
            PausePartyMode();
        }

        await Task.Delay(1000);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        EmitSignalSetGlowColor(Colors.White);
        //_material.AlbedoColor = Colors.White;
        
        _environment.SetGlowIntensity(8);
        _environment.SetGlowStrength(8);

        var tweenIntensity = GetTree().CreateTween();
        var tweenGlow = GetTree().CreateTween();

        tweenIntensity
            .TweenProperty(_environment, "glow_intensity", GlowIntensity, 2)
            .SetTrans(Tween.TransitionType.Cubic);

        tweenGlow
            .TweenProperty(_environment, "glow_strength", 1, 2)
            .SetTrans(Tween.TransitionType.Cubic);


        tweenIntensity.TweenCallback(Callable.From(() =>
        {
            ResumePartyMode();
            
            EmitSignalResetGlowColor();
        }));
    }
    public void SetGlowIntensity(float intensity)
    {
        _environment.SetGlowIntensity(intensity);
    }

    public void SetGlowStrength(float intensity)
    {
        _environment.SetGlowStrength(intensity);
    }
    public float GetGlowStrength() => _environment.GetGlowStrength();
}