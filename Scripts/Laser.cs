using Godot;

namespace Temptica.Overlay.Scripts;

public partial class Laser : RayCast3D
{
	private MeshInstance3D _beamMesh;
	private CylinderMesh _cylinderMesh;
	private StandardMaterial3D _material;
	public override void _Ready()
	{
		_beamMesh = GetNode<MeshInstance3D>("BeamMesh");
		_cylinderMesh = (CylinderMesh)_beamMesh.Mesh;
		_material = (StandardMaterial3D)_beamMesh.GetSurfaceOverrideMaterial(0);
		
	}
	
	public override void _Process(double delta)
	{
		ForceRaycastUpdate();
		if (!IsColliding())
		{
			//if not colliding, set the height to 100
			_cylinderMesh.Height = 100;
			_beamMesh.Position = _beamMesh.Position with { Y = _cylinderMesh.Height / 2 };
		};
		
		var castTo = ToLocal(GetCollisionPoint());
		_cylinderMesh.Height = castTo.Y;
		_beamMesh.Position = _beamMesh.Position with { Y = _cylinderMesh.Height / 2 };
	}
	
	public Color GetMeshColor()
	{
		return _material.AlbedoColor;
	}
	
	public void SetMeshColor(Color color)
	{
		_material.AlbedoColor = color;
	}
	
	public StandardMaterial3D GetMaterial()
	{
		return _material;
	}
	
}
