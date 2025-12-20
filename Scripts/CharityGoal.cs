using System;
using Godot;
using Temptica.Overlay.Scripts.SignalR.Listeners;

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

        GenericSignalRListener.CharityChanged += (_, tuple) =>
        {
            _currentAmount = tuple.current;
            _totalAmount = tuple.goal;

            _text = $"Charity: {_currentAmount}/{_totalAmount} {tuple.currency}";
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