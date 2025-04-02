using System;
using Godot;
using Temptic404Overlay.Scripts.SignalR.Listeners.GameListeners;
using Temptic404Overlay.Templates;

namespace Temptic404Overlay.Scripts;

public partial class PlushySpawner : Node3D
{
		[Export] public Node3D Target;
    	private double _timeTillNextSpawn;
    	private PackedScene _throwable;
    
    	private int _itemsToSpawn;
    	
    	// Called when the node enters the scene tree for the first time.
    	public override void _Ready()
    	{
    		_throwable = GD.Load<PackedScene>("res://Templates/godot_plushie.tscn");
    
    		ThrowPlushieListener.OnThrowPlushie += (_, _) =>
    		{
    			_itemsToSpawn++;
    		};
    	}
    
    	// Called every frame. 'delta' is the elapsed time since the previous frame.
    	public override void _Process(double delta)
    	{
    		_timeTillNextSpawn += delta;

		    if (_itemsToSpawn <= 0) return;
		    
		    var plushie = _throwable.Instantiate<Plushie>();
		    AddChild(plushie);
    
		    var height = new Random().Next(75, 300) / 100f;
    
		    var targetHeight = new Vector3(Target.GlobalPosition.X, (float)new Random().NextDouble()*2f+Target.GlobalPosition.Y, Target.GlobalPosition.Z);
		    var velocity = CalculateShotVelocity(plushie.GlobalPosition, targetHeight, height);
		    plushie.LinearVelocity = velocity;
		    _timeTillNextSpawn = 0;
		    _itemsToSpawn--;
	    }
    
    	private static Vector3 CalculateShotVelocity(Vector3 from, Vector3 to, float additionalHeight) {
    
    		var maxHeight = Mathf.Max(from.Y, to.Y) + additionalHeight;
    		var verticalVelocity = Mathf.Sqrt((maxHeight - from.Y) * 2 * 9.8f);
    
    		var peakTime = verticalVelocity / 9.8f;
    		var fallTime = Mathf.Sqrt((maxHeight - to.Y) * 2 / 9.8f);
    		var totalTime = peakTime + fallTime;
    
    		var positionDelta = to - from;
    		positionDelta.Y = 0;
    
    		var distance = positionDelta.Length();
    
    		var finalVelocity = positionDelta.Normalized() * (distance / totalTime);
    		finalVelocity.Y = verticalVelocity;
    
    		return finalVelocity;
    	}
}