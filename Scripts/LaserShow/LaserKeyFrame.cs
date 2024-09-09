using Godot;

namespace Temptic404Overlay.Scripts.LaserShow;

public class LaserKeyFrame
{
    public int LaserId { get; set; }
    public bool On { get; set; }
    public Vector3 Rotation { get; set; }
    public string Color { get; set; }
    public float Time { get; set; }
    public float Duration { get; set; }
}