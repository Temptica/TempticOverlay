using System;
using Godot;
using Godot.Collections;

namespace Temptica.Overlay.Scripts;

public partial class DragAndDropMovement : Node3D
{
    private Vector3 _mousePosition;
    private bool _doDrag;
    private CollisionObject3D _draggingCollider;
    private bool _isRotate;
    private bool _wasRotating;

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouse mouseEvent) return;

        var intersect = GetMouseIntersect(mouseEvent.Position);
        if (intersect.TryGetValue("position", out var position))
        {
            _mousePosition = position.AsVector3();
        }

        if (@event is not InputEventMouseButton mouseButtonEvent) return;

        switch (mouseButtonEvent.ButtonIndex)
        {
            case MouseButton.Left when mouseButtonEvent.Pressed:
                _doDrag = true;
                DragAndDrop(intersect);
                break;
            case MouseButton.Left when !mouseButtonEvent.Pressed:
                _doDrag = false;
                DragAndDrop(intersect);
                break;
            case MouseButton.Right when mouseButtonEvent.IsDoubleClick():
                _isRotate = true;
                break;
            case MouseButton.Right when !mouseButtonEvent.IsDoubleClick():
                _isRotate = false;
                _wasRotating = false;
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (_draggingCollider == null) return;
        
        _draggingCollider.GlobalPosition = new Vector3(_mousePosition.X, _mousePosition.Y, -0.1f);

        if (_draggingCollider is Otter otter)
        {
            otter.OriginalPosition = _draggingCollider.GlobalPosition;
        }

        if (!_isRotate || _wasRotating) return;
        
        _wasRotating = true;
        _draggingCollider.RotateZ(float.DegreesToRadians(90f));
        //if Z rotation is not a %90, then round it to the closest 90° angle
        // if (_draggingCollider.RotationDegrees.Z % 90 == 0) return;
        //
        // var closestZ = Math.Round(_draggingCollider.RotationDegrees.Z / 90);
        // _draggingCollider.RotationDegrees = new Vector3(_draggingCollider.RotationDegrees.X,_draggingCollider.RotationDegrees.Y,(float)closestZ*90f);
    }

    private void DragAndDrop(Dictionary intersect)
    {
        if (_doDrag)
        {
            if (_draggingCollider == null && intersect.TryGetValue("collider", out var collider))
            {
                if (collider.Obj is not CollisionObject3D colliderObj)
                {
                    return;
                }

                _draggingCollider = colliderObj;
                _draggingCollider.SetCollisionLayerValue(14, false);

                return;
            }
        }

        if (_draggingCollider == null) return;

        _draggingCollider.SetCollisionLayerValue(14, true);
        _draggingCollider = null;
    }

    private Dictionary GetMouseIntersect(Vector2 mousePosition)
    {
        var currentCamera = GetViewport().GetCamera3D();
        var param = new PhysicsRayQueryParameters3D
        {
            From = currentCamera.ProjectRayOrigin(mousePosition),
            To = currentCamera.ProjectPosition(mousePosition, 1000)
        };

        var worldSpace = GetWorld3D().DirectSpaceState;
        var result = worldSpace.IntersectRay(param);

        return result;
    }
}