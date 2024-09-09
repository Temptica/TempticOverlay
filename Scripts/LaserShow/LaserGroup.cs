using System.Collections.Generic;
using Godot;

namespace Temptic404Overlay.Scripts.LaserShow;

public class LaserGroup
{
    public int GroupId { get; set; }
    public List<ShowLaser> Lasers { get; set; }
    public Vector3 Position { get; set; }
}