using System;
using TMPro;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class XRInteractionController : MonoBehaviour
{
    [System.Serializable]
    public enum InteractionMode
    {
        Idle,
        FacadeEditing,
        WindowAddition,
        WindowRemoval,
        WindowsLayoutEditing,
        FloorEditing,
        AdditionalFloorEditing,
        PushAndPull
    }

    public InteractionMode mode;

    private int totalModes = Enum.GetNames(typeof(InteractionMode)).Length;

    public GameObject modeText;

    Transform raycastHitTransform;

    Building building;

    public GameObject alignment, facadeEditing, floorEditing, otherEditing, floorCurtainChange;

    public Window selectedWindow,
        pendingMergedWindow,
        pendingAddWindow;

    public Floor selectedFloor;

    bool pressed;

    public GameObject testCube;

    public GameObject pullObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        building = FindObjectOfType<Building>();
        mode = InteractionMode.Idle;
        modeText.GetComponent<TextMeshPro>().text = mode.ToString();

        alignment.SetActive(true);
        facadeEditing.SetActive(false);
        floorEditing.SetActive(false);
        otherEditing.SetActive(false);
    }

    // public void SwitchMode(bool up)
    // {
    //     selectedWindow = null;
    //     pendingMergedWindow = null;
    //     if (pendingAddWindow)
    //         Destroy(pendingAddWindow.gameObject);
    //     pendingAddWindow = null;

    //     if (up)
    //     {
    //         switch (mode)
    //         {
    //             case InteractionMode.WindowAddition:
    //                 mode = InteractionMode.WindowRemoval;
    //                 break;
    //             case InteractionMode.WindowRemoval:
    //                 mode = InteractionMode.WindowsLayoutEditing;
    //                 break;
    //             case InteractionMode.WindowsLayoutEditing:
    //                 mode = InteractionMode.FloorEditing;
    //                 break;
    //             case InteractionMode.FloorEditing:
    //                 mode = InteractionMode.FacadeEditing;
    //                 break;
    //             case InteractionMode.FacadeEditing:
    //                 mode = InteractionMode.WindowAddition;
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    //     else
    //     {
    //         switch (mode)
    //         {
    //             case InteractionMode.WindowAddition:
    //                 mode = InteractionMode.FacadeEditing;
    //                 break;
    //             case InteractionMode.WindowRemoval:
    //                 mode = InteractionMode.WindowAddition;
    //                 break;
    //             case InteractionMode.WindowsLayoutEditing:
    //                 mode = InteractionMode.WindowRemoval;
    //                 break;
    //             case InteractionMode.FloorEditing:
    //                 mode = InteractionMode.WindowsLayoutEditing;
    //                 break;
    //             case InteractionMode.FacadeEditing:
    //                 mode = InteractionMode.FloorEditing;
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }

    // public void UpdateRaycastResult(Ray ray)
    // {
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, 500f))
    //         raycastHitTransform = hit.transform;
    //     else
    //         raycastHitTransform = null;

    //     switch (mode)
    //     {
    //         // carton migrated
    //         case InteractionMode.WindowAddition:
    //             if (raycastHitTransform != null)
    //             {
    //                 if (raycastHitTransform.parent.GetComponent<Floor>())
    //                 {
    //                     Floor f = raycastHitTransform.parent.GetComponent<Floor>();

    //                     if (pendingAddWindow == null)
    //                     {
    //                         pendingAddWindow = Instantiate(f.windowPrefab).GetComponent<Window>();
    //                         pendingAddWindow.gameObject.transform.parent = f.transform;
    //                         pendingAddWindow.TurnOffCollider();
    //                     }

    //                     Vector3 relativePos = hit.point - f.gameObject.transform.position;
    //                     if (raycastHitTransform == f.front)
    //                     {
    //                         pendingAddWindow.gameObject.transform.localPosition = new Vector3(
    //                             -35.51f,
    //                             5f,
    //                             relativePos.z
    //                         );
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (pendingAddWindow)
    //                     {
    //                         Destroy(pendingAddWindow.gameObject);
    //                         pendingAddWindow = null;
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 if (pendingAddWindow)
    //                 {
    //                     Destroy(pendingAddWindow.gameObject);
    //                     pendingAddWindow = null;
    //                 }
    //             }
    //             break;
    //         case InteractionMode.WindowRemoval:
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowsLayoutEditing:
    //             if (selectedWindow)
    //             {
    //                 pendingMergedWindow = selectedWindow.MoveWindow(ray);
    //             }
    //             else
    //             {
    //                 pendingMergedWindow = null;
    //             }
    //             break;
    //         case InteractionMode.FloorEditing:
    //             break;
    //         case InteractionMode.FacadeEditing:
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // @Carton
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    // @Carton
    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            var spatialPointerState = EnhancedSpatialPointerSupport.GetPointerState(
                Touch.activeTouches[0]
            );

            // Ignore poke input--piece will get stuck to the user's finger
            if (spatialPointerState.Kind == SpatialPointerKind.Touch)
            {
                return;
            }

            switch (mode)
            {
                case InteractionMode.Idle:
                    switch (spatialPointerState.phase)
                    {
                        case SpatialPointerPhase.Ended:
                        case SpatialPointerPhase.Cancelled:
                            if (spatialPointerState.targetObject != null)
                            {
                                building.ClearPlan();
                            }
                            break;
                    }
                    break;
                case InteractionMode.WindowAddition:
                    switch (spatialPointerState.phase)
                    {
                        case SpatialPointerPhase.Began:
                            if (spatialPointerState.targetObject != null)
                            {
                                if (
                                    spatialPointerState.targetObject.transform.parent.GetComponent<Floor>()
                                )
                                {
                                    selectedFloor =
                                        spatialPointerState.targetObject.transform.parent.GetComponent<Floor>();

                                    if (pendingAddWindow == null)
                                    {
                                        pendingAddWindow = Instantiate(selectedFloor.windowPrefab)
                                            .GetComponent<Window>();
                                        pendingAddWindow.gameObject.transform.parent =
                                            selectedFloor.transform;
                                        pendingAddWindow.TurnOffCollider();
                                    }

                                    // Debug.Log(
                                    //     "spatial pointer interaction pos "
                                    //         + spatialPointerState.interactionPosition
                                    // );
                                    if (
                                        spatialPointerState.targetObject.transform
                                        == selectedFloor.front
                                    )
                                    {
                                        pendingAddWindow.gameObject.transform.position =
                                            spatialPointerState.interactionPosition;
                                        pendingAddWindow.gameObject.transform.localPosition =
                                            new Vector3(
                                                -35.61f,
                                                5f,
                                                pendingAddWindow
                                                    .gameObject
                                                    .transform
                                                    .localPosition
                                                    .z
                                            );
                                        pendingAddWindow.gameObject.transform.localScale =
                                            new Vector3(1, 1, 1);
                                        pendingAddWindow.gameObject.transform.localRotation =
                                            Quaternion.identity;
                                    }
                                }
                                else
                                {
                                    if (pendingAddWindow)
                                    {
                                        Destroy(pendingAddWindow.gameObject);
                                        pendingAddWindow = null;
                                    }
                                }
                            }
                            else
                            {
                                if (pendingAddWindow)
                                {
                                    Destroy(pendingAddWindow.gameObject);
                                    pendingAddWindow = null;
                                }
                            }
                            break;
                        case SpatialPointerPhase.Moved:
                            if (pendingAddWindow)
                            {
                                Vector3 originalPosition = pendingAddWindow
                                    .gameObject
                                    .transform
                                    .localPosition;
                                pendingAddWindow.gameObject.transform.localPosition = new Vector3(
                                    -35.61f,
                                    5f,
                                    originalPosition.z
                                        + spatialPointerState.deltaInteractionPosition.x
                                );
                            }
                            break;
                        case SpatialPointerPhase.Ended:
                        case SpatialPointerPhase.Cancelled:
                            if (spatialPointerState.targetObject != null)
                            {
                                if (
                                    spatialPointerState.targetObject.transform.parent.GetComponent<Floor>()
                                )
                                {
                                    Debug.Log(
                                        "Add window mode name of targetObject: "
                                            + spatialPointerState.targetObject.name
                                    );
                                    if (
                                        spatialPointerState.targetObject.transform
                                        == selectedFloor.front
                                    )
                                    {
                                        if (pendingAddWindow)
                                        {
                                            selectedFloor.AddWindow(
                                                0,
                                                pendingAddWindow.gameObject.transform.localPosition
                                            );
                                        }
                                    }
                                }
                                if (pendingAddWindow)
                                {
                                    Destroy(pendingAddWindow.gameObject);
                                    pendingAddWindow = null;
                                }
                            }
                            break;
                    }
                    break;
                case InteractionMode.WindowRemoval:
                    switch (spatialPointerState.phase)
                    {
                        case SpatialPointerPhase.Ended:
                        case SpatialPointerPhase.Cancelled:
                            if (spatialPointerState.targetObject != null)
                            {
                                if (
                                    spatialPointerState.targetObject.transform.parent.GetComponent<Window>()
                                )
                                {
                                    selectedWindow =
                                        spatialPointerState.targetObject.transform.parent.GetComponent<Window>();
                                    selectedFloor = selectedWindow.floor;
                                    selectedFloor.RemoveWindow(selectedWindow);
                                    selectedWindow = null;
                                }
                            }
                            break;
                    }
                    break;
                case InteractionMode.WindowsLayoutEditing:
                    switch (spatialPointerState.phase)
                    {
                        case SpatialPointerPhase.Began:
                            if (spatialPointerState.targetObject != null)
                            {
                                if (
                                    spatialPointerState.targetObject.transform.parent.GetComponent<Window>()
                                )
                                {
                                    Debug.Log(
                                        "targetObject: " + spatialPointerState.targetObject.name
                                    );
                                    selectedWindow =
                                        spatialPointerState.targetObject.transform.parent.GetComponent<Window>();
                                    selectedFloor = selectedWindow.floor;
                                }
                            }
                            break;
                        case SpatialPointerPhase.Moved:
                            // selectedWindow.MoveWindow(spatialPointerState.interactionPosition);
                            if (
                                spatialPointerState.targetObject.transform.parent.GetComponent<Window>()
                            )
                            {
                                // if (Touch.activeTouches.Count == 1)
                                // {
                                //     Vector3 originalPosition = selectedWindow
                                //         .gameObject
                                //         .transform
                                //         .localPosition;
                                //     selectedWindow.gameObject.transform.localPosition = new Vector3(
                                //         -35.61f,
                                //         5f,
                                //         originalPosition.z
                                //             - spatialPointerState.deltaInteractionPosition.x
                                //     );
                                // }
                                // @Carton need adjustments for the parameters here
                                // if (Touch.activeTouches.Count == 1)
                                // {
                                //     var secondSpatialPointerState =
                                //         EnhancedSpatialPointerSupport.GetPointerState(
                                //             Touch.activeTouches[1]
                                //         );
                                selectedWindow.ScaleWindow(
                                    -spatialPointerState.deltaInteractionPosition.x
                                );
                            }
                            break;
                    }
                    break;
                case InteractionMode.FloorEditing:
                    // switch (spatialPointerState.phase)
                    // {
                    //     case SpatialPointerPhase.Cancelled:
                    //     case SpatialPointerPhase.Ended:
                    //         building.DanggleFloor();
                    //         break;
                    // }
                    break;
                // case InteractionMode.FacadeEditing:
                // switch (spatialPointerState.phase)
                // {
                //     case SpatialPointerPhase.Cancelled:
                //     case SpatialPointerPhase.Ended:
                //         if (building)
                //         {
                //             // @Carton
                //             building.SwitchUpPlan();
                //         }
                //         break;
                // }
                // break;
                case InteractionMode.PushAndPull:
                    switch (spatialPointerState.phase)
                    {
                        case SpatialPointerPhase.Moved:
                            Vector3 originalPosition = pullObject.transform.localPosition;
                            pullObject.transform.localPosition = new Vector3(
                                originalPosition.x,
                                originalPosition.y,
                                Mathf.Clamp(
                                    originalPosition.z
                                        + spatialPointerState.deltaInteractionPosition.z,
                                    -16f,
                                    16f
                                )
                            );
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // public void UpdateInputValue(float delta, float acceleration)
    // {
    //     switch (mode)
    //     {
    //         case InteractionMode.WindowAddition:
    //             break;
    //         case InteractionMode.WindowRemoval:
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowsLayoutEditing:
    //             if (selectedWindow)
    //             {
    //                 selectedWindow.ScaleWindow(delta * acceleration);
    //             }
    //             break;
    //         // carton migrated
    //         case InteractionMode.FloorEditing:
    //             if (building)
    //             {
    //                 if (delta > .99f)
    //                     building.AddFloor();
    //                 else if (delta < -.99f)
    //                     building.RemoveFloor();
    //             }
    //             break;
    //         // carton migrated
    //         case InteractionMode.FacadeEditing:
    //             if (building)
    //             {
    //                 if (delta > .99f)
    //                     building.SwitchUpPlan();
    //                 else if (delta < -.99f)
    //                     building.SwitchDownPlan();
    //             }
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // public void UpdateInputButton()
    // {
    //     switch (mode)
    //     {
    //         case InteractionMode.WindowAddition:
    //             break;
    //         case InteractionMode.WindowRemoval:
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowsLayoutEditing:
    //             if (selectedWindow && pendingMergedWindow)
    //             {
    //                 selectedWindow.floor.MergeWindow(selectedWindow, pendingMergedWindow);
    //             }
    //             break;
    //         case InteractionMode.FloorEditing:
    //             break;
    //         case InteractionMode.FacadeEditing:
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // public void OnPress()
    // {
    //     pressed = true;

    //     switch (mode)
    //     {
    //         // carton migrated
    //         case InteractionMode.WindowAddition:
    //             if (raycastHitTransform != null)
    //             {
    //                 if (raycastHitTransform.parent.GetComponent<Floor>())
    //                 {
    //                     Floor f = raycastHitTransform.parent.GetComponent<Floor>();
    //                     if (raycastHitTransform == f.front)
    //                     {
    //                         if (pendingAddWindow)
    //                         {
    //                             f.AddWindow(0, pendingAddWindow.gameObject.transform.localPosition);
    //                         }
    //                     }
    //                 }
    //             }
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowRemoval:
    //             if (raycastHitTransform != null)
    //             {
    //                 selectedWindow = raycastHitTransform.parent.GetComponent<Window>();
    //                 if (selectedWindow)
    //                 {
    //                     Floor f = selectedWindow.floor;
    //                     f.RemoveWindow(selectedWindow);
    //                     selectedWindow = null;
    //                 }
    //             }
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowsLayoutEditing:
    //             if (raycastHitTransform != null)
    //             {
    //                 selectedWindow = raycastHitTransform.parent.GetComponent<Window>();
    //             }
    //             break;
    //         case InteractionMode.FloorEditing:
    //             break;
    //         case InteractionMode.FacadeEditing:
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // public void OnRelease()
    // {
    //     pressed = false;

    //     switch (mode)
    //     {
    //         case InteractionMode.WindowAddition:
    //             break;
    //         case InteractionMode.WindowRemoval:
    //             break;
    //         // carton migrated
    //         case InteractionMode.WindowsLayoutEditing:
    //             selectedWindow = null;
    //             break;
    //         case InteractionMode.FloorEditing:
    //             break;
    //         case InteractionMode.FacadeEditing:
    //             break;
    //         default:
    //             break;
    //     }
    // }

    public void Checkerboard()
    {
        if (building)
        {
            float[] firstFloor = new float[9];
            for (int i = 1; i < 10; i++)
                firstFloor[i - 1] = (float)i / 10f;
            float[] secondFloor = new float[8];
            for (int i = 1; i < 9; i++)
                secondFloor[i - 1] = (float)i / 10f + .05f;
            float[] thirdFloor = new float[9];
            for (int i = 1; i < 10; i++)
                thirdFloor[i - 1] = (float)i / 10f;
            building.floors[0].SetLayout(0, firstFloor);
            building.floors[1].SetLayout(0, secondFloor);
            building.floors[2].SetLayout(0, thirdFloor);
        }
    }

    public void CopyPatterns()
    {
        if (building)
        {
            building.CopyWindowsPatterns(0, 1);
            building.CopyWindowsPatterns(0, 2);
        }
    }

    // @Carton
    public void SwitchToNextMode()
    {
        int currentIndex = (int)mode;

        int nextIndex = (currentIndex + 1) % totalModes;

        mode = (InteractionMode)nextIndex;

        modeText.GetComponent<TextMeshPro>().text = mode.ToString();

        Debug.Log($"Switched to mode: {mode}");

        switch (mode)
        {
            case InteractionMode.FacadeEditing:
                facadeEditing.SetActive(true);
                otherEditing.SetActive(false);
                floorEditing.SetActive(false);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.WindowsLayoutEditing:
                floorEditing.SetActive(false);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(true);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.WindowAddition:
                floorEditing.SetActive(false);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(true);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.WindowRemoval:
                floorEditing.SetActive(false);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(true);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.FloorEditing:
                floorCurtainChange.SetActive(true);
                floorEditing.SetActive(false);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(false);
                alignment.SetActive(false);
                break;
            case InteractionMode.AdditionalFloorEditing:
                floorEditing.SetActive(true);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(false);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.PushAndPull:
                floorEditing.SetActive(true);
                facadeEditing.SetActive(false);
                otherEditing.SetActive(false);
                alignment.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            case InteractionMode.Idle:
                alignment.SetActive(true);
                facadeEditing.SetActive(false);
                floorEditing.SetActive(false);
                otherEditing.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
            default:
                alignment.SetActive(false);
                facadeEditing.SetActive(false);
                floorEditing.SetActive(false);
                otherEditing.SetActive(false);
                floorCurtainChange.SetActive(false);
                break;
        }
    }
}
