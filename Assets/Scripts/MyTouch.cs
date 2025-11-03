using System.Collections.Generic;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using PolySpatial.Samples;

/// <summary>
/// Current you can only select one object at a time and only supports a primary [0] touch
/// </summary>
public class MyTouch : MonoBehaviour
{
    struct Selection
    {
        public MySelectable Piece;
        public Vector3 PositionOffset;
        public Quaternion RotationOffset;
    }

    public const int k_Deselected = -1;

    readonly Dictionary<int, Selection> m_SelectedBuilding = new();

    // private Vector3? lastHandPinchPosition = null;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    bool m_canMove = true;
    public void ToggleMove()
    {
        m_canMove = !m_canMove;
    }

    void Update()
    {
        if(!m_canMove) return;
        
        foreach (var touch in Touch.activeTouches)
        {
            var spatialPointerState = EnhancedSpatialPointerSupport.GetPointerState(touch);
            var interactionId = spatialPointerState.interactionId;

            // Ignore poke input--piece will get stuck to the user's finger
            if (spatialPointerState.Kind == SpatialPointerKind.Touch)
                continue;

            var pieceObject = spatialPointerState.targetObject;
            // Debug.Log($"Touch {touch.touchId} {spatialPointerState.phase} {pieceObject} {spatialPointerState.Kind} {spatialPointerState.interactionPosition} {spatialPointerState.inputDeviceRotation}");
            
            // @Carton: the following if clause code is not working since we did not add the Myselctable component to the object

            // if (pieceObject != null)
            // {
            //     // Swap materials and record initial relative position & rotation from hand to object for later use when the piece is selected
            //     if (pieceObject.TryGetComponent(out MySelectable piece) &&
            //             piece.selectingPointer == k_Deselected) {
            //         Debug.Log("Contains Myselctable component");
            //         var pieceTransfor = piece.transform.parent.transform.parent.transform;
                    
            //         // spatialPointerState.Kind == SpatialPointerKind.DirectPinch
            //         //     ? piece.transform.parent.transform
            //         //     : piece.transform.parent.transform.parent.transform;
            //         var interactionPosition = spatialPointerState.interactionPosition;
            //         var inverseDeviceRotation = Quaternion.Inverse(spatialPointerState.inputDeviceRotation);
                    
            //         var rotationOffset = inverseDeviceRotation * pieceTransfor.rotation;
            //         var positionOffset = (pieceTransfor.position - interactionPosition);
            //         piece.SetSelected(interactionId);

            //         // Because events can come in faster than they are consumed, it is possible for target id to change without a prior end/cancel event
            //         if (m_SelectedBuilding.TryGetValue(interactionId, out var select))
            //             select.Piece.SetSelected(k_Deselected);

            //         m_SelectedBuilding[interactionId] = new Selection
            //         {
            //             Piece = piece,
            //             PositionOffset = positionOffset,
            //             RotationOffset = rotationOffset
            //         };
            //     }
            // }

            // @Carton: we don't need the following code
            // switch (spatialPointerState.phase)
            // {
            //     case SpatialPointerPhase.Began:
            //         break;
            //     case SpatialPointerPhase.Moved:
            //         if (m_SelectedBuilding.TryGetValue(interactionId, out var selection))
            //         {
            //             var deviceRotation = spatialPointerState.inputDeviceRotation;
            //             var rotation = deviceRotation * selection.RotationOffset;
            //             var newPosition = spatialPointerState.interactionPosition + selection.PositionOffset;
            //             // newPosition = new Vector3(newPosition.x, 0, newPosition.z);
            //             // selectedBuilding.Building.transform.parent.SetPositionAndRotation(newPosition, rotation);
            //             // selection.Piece.transform.parent.transform.SetPositionAndRotation(newPosition, rotation);
            //             // if(spatialPointerState.Kind == SpatialPointerKind.DirectPinch)
            //                 // selection.Piece.transform.parent.transform.position = newPosition;
            //             // else
            //             selection.Piece.transform.parent.transform.parent.transform.position = newPosition;
            //         }
            //         break;
            //     case SpatialPointerPhase.None:
            //     case SpatialPointerPhase.Ended:
            //     case SpatialPointerPhase.Cancelled:
            //         DeselectBuilding(interactionId);
            //         break;
            // }

            // switch (spatialPointerState.phase)
            // {
            //     case SpatialPointerPhase.Began:
            //         lastHandPinchPosition = spatialPointerState.inputDevicePosition;
            //         break;
            //     case SpatialPointerPhase.Moved:
            //         if(pieceObject.name == "Cube"){
            //             if (lastHandPinchPosition.HasValue)
            //             {
            //                 Vector3 handPinchPosition = spatialPointerState.inputDevicePosition;
            //                 float deltaX = handPinchPosition.x - lastHandPinchPosition.Value.x;
            //                 float deltaY = handPinchPosition.y - lastHandPinchPosition.Value.y;
            //                 float deltaZ = handPinchPosition.z - lastHandPinchPosition.Value.z;

            //                 // Update the position based on the delta changes
            //                 pieceObject.transform.position += new Vector3(deltaX * 10, deltaY * 10, deltaZ * 10);
            //             }
            //         }
            //         lastHandPinchPosition = spatialPointerState.inputDevicePosition; // Update last position
            //         break;
            //     case SpatialPointerPhase.None:
            //     case SpatialPointerPhase.Ended:
            //         lastHandPinchPosition = null;
            //         break;
            //     case SpatialPointerPhase.Cancelled:
            //         lastHandPinchPosition = null;
            //         break;
            // }
        }
    }


    void DeselectBuilding(int interactionId)
    {
        if (m_SelectedBuilding.TryGetValue(interactionId, out var selectedBuilding))
        {
            // Swap materials back when the piece is deselected
            selectedBuilding.Piece.SetSelected(k_Deselected);
            m_SelectedBuilding.Remove(interactionId);
        }
    }
}
