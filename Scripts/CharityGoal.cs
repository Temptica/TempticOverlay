using System;
using Godot;
using TwitcherSharp.Api.Generated.Charity;
using TwitcherSharp.EventSub;
using TwitcherSharp.EventSub.Generated.CharityCampaignProgress;


namespace Temptica.Overlay.Scripts;

public partial class CharityGoal : Node3D
{
    [Export] private MeshInstance3D _mesh = null!;
    [Export] private Label3D _label = null!;

    private double _currentAmount;
    private double _totalAmount;
    private string _text;
    private bool _changed;

    public override void _Ready()
    {
        //listen to signalR
        //first send a request to get the time until the next ad
        
        var eventListener = new TwitchEventListener<TwitchCharityCampaignProgressEvent>();
        
        eventListener.SubscriptionDefinition = TwitchEventSubDefinition.ChannelCharityCampaignProgress;
        
        AddChild(eventListener.ToGodotObject() as Node);
        
        eventListener.Received += eventData =>
        {
            
            _currentAmount = eventData.CurrentAmount.Value / Math.Pow(10f,eventData.CurrentAmount.DecimalPlaces);
            _totalAmount = eventData.TargetAmount.Value / Math.Pow(10f,eventData.TargetAmount.DecimalPlaces);

            _text = $"Charity: {_currentAmount}/{_totalAmount} {eventData.CurrentAmount.Currency}";
            _changed = true;
        };
    }

    public override void _Process(double delta)
    {
        if (!_changed) return;
        const int originalSize = 4;

        _label.Text = _text;

        var mesh = (BoxMesh)_mesh.Mesh;

        var percent = _currentAmount / _totalAmount;

        percent = Math.Clamp(percent, 0, 1);


        mesh.Size = new Vector3((float)(originalSize * percent), mesh.Size.Y, mesh.Size.Z);

        _mesh.Position = new Vector3((mesh.Size.X - originalSize) / 2f, 0, 0);

        _changed = false;
    }
}