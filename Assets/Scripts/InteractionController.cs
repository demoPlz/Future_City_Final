using UnityEngine;
using System;

public class InteractionController : MonoBehaviour
{
    [System.Serializable]
    public enum InteractionMode
    { 
        WindowAddition,
        WindowRemoval,
        WindowsLayoutEditing,
        FloorEditing,
        FacadeEditing,
    }

    public InteractionMode mode;

    Transform raycastHitTransform;

    Building building;

    Window selectedWindow, pendingMergedWindow, pendingAddWindow, pendingRemovalWindow;

    bool pressed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        building = FindObjectOfType<Building>();
        mode = InteractionMode.WindowAddition;
    }

    public void SwitchMode(bool up)
    {
        selectedWindow = null;
        pendingMergedWindow = null;
        if (pendingAddWindow)
            Destroy(pendingAddWindow.gameObject);
        pendingAddWindow = null;
        if (pendingRemovalWindow)
            pendingRemovalWindow.SetOnSelectedVisual(false);
        pendingRemovalWindow = null;

        if (up)
        {
            switch (mode)
            {
                case InteractionMode.WindowAddition:
                    mode = InteractionMode.WindowRemoval;
                    break;
                case InteractionMode.WindowRemoval:
                    mode = InteractionMode.WindowsLayoutEditing;
                    break;
                case InteractionMode.WindowsLayoutEditing:
                    mode = InteractionMode.FloorEditing;
                    break;
                case InteractionMode.FloorEditing:
                    mode = InteractionMode.FacadeEditing;
                    break;
                case InteractionMode.FacadeEditing:
                    mode = InteractionMode.WindowAddition;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case InteractionMode.WindowAddition:
                    mode = InteractionMode.FacadeEditing;
                    break;
                case InteractionMode.WindowRemoval:
                    mode = InteractionMode.WindowAddition;
                    break;
                case InteractionMode.WindowsLayoutEditing:
                    mode = InteractionMode.WindowRemoval;
                    break;
                case InteractionMode.FloorEditing:
                    mode = InteractionMode.WindowsLayoutEditing;
                    break;
                case InteractionMode.FacadeEditing:
                    mode = InteractionMode.FloorEditing;
                    break;
                default:
                    break;
            }
        }
    }

    public void UpdateRaycastResult(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500f))
            raycastHitTransform = hit.transform;
        else
            raycastHitTransform = null;

        switch (mode)
        {
            case InteractionMode.WindowAddition:
                if (raycastHitTransform != null)
                {
                    if (raycastHitTransform.parent.GetComponent<Floor>())
                    {
                        Floor f = raycastHitTransform.parent.GetComponent<Floor>();

                        if (pendingAddWindow == null)
                        {
                            pendingAddWindow = Instantiate(f.windowPrefab).GetComponent<Window>();
                            pendingAddWindow.gameObject.transform.parent = f.transform;
                            pendingAddWindow.TurnOffCollider();
                        }

                        Vector3 relativePos = hit.point - f.gameObject.transform.position;
                        if (raycastHitTransform == f.front)
                        {
                            pendingAddWindow.gameObject.transform.localPosition = new Vector3(-35.51f, 5f, relativePos.z);
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
            case InteractionMode.WindowRemoval:
                if (raycastHitTransform != null)
                {
                    if (raycastHitTransform.parent.GetComponent<Window>())
                    {
                        pendingRemovalWindow = raycastHitTransform.parent.GetComponent<Window>();
                        pendingRemovalWindow.SetOnSelectedVisual(true);
                    }
                    else
                    {
                        if (pendingRemovalWindow)
                        {
                            pendingRemovalWindow.SetOnSelectedVisual(false);
                            pendingRemovalWindow = null;
                        }
                    }
                }
                else
                {
                    if (pendingRemovalWindow)
                    {
                        pendingRemovalWindow.SetOnSelectedVisual(false);
                        pendingRemovalWindow = null;
                    }
                }
                break;
            case InteractionMode.WindowsLayoutEditing:
                if (selectedWindow)
                {
                    pendingMergedWindow = selectedWindow.MoveWindow(ray);
                }
                else
                {
                    pendingMergedWindow = null;
                }
                break;
            case InteractionMode.FloorEditing:
                break;
            case InteractionMode.FacadeEditing:
                break;
            default:
                break;
        }
    }

    public void UpdateInputValue(float delta, float acceleration)
    {
        switch (mode)
        {
            case InteractionMode.WindowAddition:
                break;
            case InteractionMode.WindowRemoval:
                break;
            case InteractionMode.WindowsLayoutEditing:
                if (selectedWindow)
                {
                    selectedWindow.ScaleWindow(delta * acceleration);
                }
                break;
            case InteractionMode.FloorEditing:
                if (building)
                {
                    if (delta > .99f) building.AddFloor();
                    else if (delta < -.99f) building.RemoveFloor();
                }
                break;
            case InteractionMode.FacadeEditing:
                if (building)
                {
                    if (delta > .99f) building.SwitchUpPlan();
                    else if (delta < -.99f) building.SwitchDownPlan();
                }
                break;
            default:
                break;
        }
    }

    public void UpdateInputButton()
    {
        switch (mode)
        {
            case InteractionMode.WindowAddition:
                break;
            case InteractionMode.WindowRemoval:
                break;
            case InteractionMode.WindowsLayoutEditing:
                if (selectedWindow && pendingMergedWindow)
                {
                    selectedWindow.floor.MergeWindow(selectedWindow, pendingMergedWindow);
                }
                break;
            case InteractionMode.FloorEditing:
                break;
            case InteractionMode.FacadeEditing:
                break;
            default:
                break;
        }
    }

    public void OnPress()
    {
        pressed = true;

        switch (mode)
        {
            case InteractionMode.WindowAddition:
                if (raycastHitTransform != null)
                {
                    if (raycastHitTransform.parent.GetComponent<Floor>())
                    {
                        Floor f = raycastHitTransform.parent.GetComponent<Floor>();
                        if (raycastHitTransform == f.front)
                        {
                            if (pendingAddWindow)
                            {
                                f.AddWindow(0, pendingAddWindow.gameObject.transform.localPosition);
                            }
                        }
                    }
                }
                break;
            case InteractionMode.WindowRemoval:
                if (pendingRemovalWindow)
                {
                    Floor f = pendingRemovalWindow.floor;
                    f.RemoveWindow(pendingRemovalWindow);
                    pendingRemovalWindow = null;
                }
                break;
            case InteractionMode.WindowsLayoutEditing:
                if (raycastHitTransform != null)
                {
                    selectedWindow = raycastHitTransform.parent.GetComponent<Window>();
                }
                break;
            case InteractionMode.FloorEditing:
                break;
            case InteractionMode.FacadeEditing:
                break;
            default:
                break;
        }
    }

    public void OnRelease()
    {
        pressed = false;

        switch (mode)
        {
            case InteractionMode.WindowAddition:
                break;
            case InteractionMode.WindowRemoval:
                break;
            case InteractionMode.WindowsLayoutEditing:
                selectedWindow = null;
                break;
            case InteractionMode.FloorEditing:
                break;
            case InteractionMode.FacadeEditing:
                break;
            default:
                break;
        }
    }

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

    public void RestoreInitLayout()
    {
        if (building) building.LoadBuildingLayout(0);
    }
}
