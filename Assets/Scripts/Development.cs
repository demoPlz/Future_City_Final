using System.Collections.Generic;
using TMPro;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Development : MonoBehaviour
{
    public List<GameObject> modelsToRegister;
    public List<GameObject> wallsToAlign;
    public string wallDirection = "up";
    public string planeDirection = "up";

    readonly Dictionary<int, PlaneToAlign> m_SelectedPlane = new();

    internal const int k_Deselected = -1;

    public int currentAlignIndex = 0; // 新增align model索引变量

    private GameObject planeSelectText;
    private TextMeshPro planeText;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Start()
    {

    }

    void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            var spatialPointerState = EnhancedSpatialPointerSupport.GetPointerState(touch);
            var interactionId = spatialPointerState.interactionId;

            // Ignore poke input--piece will get stuck to the user's finger
            if (spatialPointerState.Kind == SpatialPointerKind.Touch)
                continue;

            var pieceObject = spatialPointerState.targetObject;
            switch (spatialPointerState.phase)
            {
                case SpatialPointerPhase.None:
                case SpatialPointerPhase.Ended:
                case SpatialPointerPhase.Cancelled:
                    if (pieceObject != null && pieceObject.TryGetComponent(out PlaneToAlign plane))
                    {
                        if (plane.selectingPointer == k_Deselected)
                        {
                            plane.SetSelected(interactionId);
                            // Because events can come in faster than they are consumed, it is possible for target id to change without a prior end/cancel event
                            if (m_SelectedPlane.TryGetValue(interactionId, out var select))
                                select.SetSelected(k_Deselected);

                            m_SelectedPlane[interactionId] = plane;
                        }
                        else
                        {
                            DeselectPlane(interactionId);
                        }
                    }
                    break;
            }
        }
    }

    Vector3 GetGeometryCenter(GameObject obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogWarning("No MeshFilter found on the selectedPlane.");
            return obj.transform.position; // Fallback to the object's position
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 centroid = Vector3.zero;

        foreach (Vector3 vertex in vertices)
        {
            centroid += obj.transform.TransformPoint(vertex);
        }

        centroid /= vertices.Length;
        return centroid;
    }

    Vector3 GetBoundingBoxCenter(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No Renderer found on the selectedPlane.");
            return obj.transform.position; // Fallback to the object's position
        }

        Bounds bounds = renderer.bounds;
        return bounds.center;
    }

    // public void UpdateNavMesh(){
    //     navMeshSurface.BuildNavMesh();
    // }
    public void AlignPlane()
    {
        GameObject model = modelsToRegister[0];
        GameObject wall = wallsToAlign[currentAlignIndex];

        Debug.Log($"AlignPlane: model {model.transform.name}, wall {wall.transform.name}");
        
        //@Carton
        //  There would only be one selected plane in total
        foreach (var selectedPlane in m_SelectedPlane.Values)
        {
            if (selectedPlane.TryGetComponent(out ARPlane arPlane))
            {
                Debug.Log($"AlignPlane: ARPlane {arPlane.size}, alignment {arPlane.alignment}");
            }

            Debug.Log(
                $"AlignPlane: position {selectedPlane.transform.position}, rotation {selectedPlane.transform.rotation}"
            );
            var planePose = selectedPlane.transform;

            // Get the child transform you want to align
            Debug.Log($"AlignPlane: WallToAlign Object {wall}");

            if (wall != null)
            {
                //  up - green axis
                // right - red axis
                // forward - blue axis
                Vector3 wallNormal = wall.transform.right; // Assuming the wall's forward direction is its normal
                if (wallDirection == "up")
                {
                    wallNormal = wall.transform.up;
                }
                else if (wallDirection == "right")
                {
                    wallNormal = wall.transform.right;
                }
                else if (wallDirection == "forward")
                {
                    wallNormal = wall.transform.forward;
                }

                Vector3 planeNormal = planePose.right; // Assuming the plane's up direction is its normal
                if (planeDirection == "up")
                {
                    planeNormal = planePose.up;
                }
                else if (planeDirection == "right")
                {
                    planeNormal = planePose.right;
                }
                else if (planeDirection == "forward")
                {
                    planeNormal = planePose.forward;
                }

                // Calculate the rotation needed to align the wall normal with the plane normal
                Quaternion rotationToAlignNormals = Quaternion.FromToRotation(
                    wallNormal,
                    planeNormal
                );
                Debug.Log($"AlignPlane: rotationToAlignNormals {rotationToAlignNormals}");
                // Apply the rotation and position offset to the building
                model.transform.rotation = rotationToAlignNormals * model.transform.rotation;

                // Keep building upright while maintaining Y rotation
                Vector3 currentRotation = model.transform.rotation.eulerAngles;
                model.transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);

                // Calculate position offset
                Vector3 planeCenter = GetBoundingBoxCenter(planePose.gameObject);
                Vector3 wallCenter = wall.transform.position;

                // Calculate the position offset
                Vector3 positionOffset = planeCenter - wallCenter;

                // Apply the position offset
                model.transform.position += positionOffset;

                Debug.Log($"Final Position: {model.transform.position}");
            }
            else
            {
                Debug.LogWarning("No WallToAlign object selected.");
            }
        }
    }

    public void PlaneSelect()
    {
        Debug.Log("PlaneSelect");
        currentAlignIndex++; // 更新索引
        currentAlignIndex %= wallsToAlign.Count; // 循环索引
        planeText.text = $"{wallsToAlign[currentAlignIndex].transform.name}";
    }

    void DeselectPlane(int interactionId)
    {
        if (m_SelectedPlane.TryGetValue(interactionId, out var selectedPlane))
        {
            // Swap materials back when the piece is deselected
            selectedPlane.SetSelected(k_Deselected);
            m_SelectedPlane.Remove(interactionId);
        }
    }
}
