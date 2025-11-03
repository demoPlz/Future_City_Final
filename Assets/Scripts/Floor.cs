using UnityEngine;
using System;
using System.Collections.Generic;

public class Floor : MonoBehaviour
{
    [System.Serializable]
    public struct WindowLayout
    {
        public List<Window> frontWindows, backWindows, leftWindows, rightWindows;
    }

    public GameObject obj;

    public float floorLength, floorWidth, floorHeight;

    public Transform front, back, left, right;

    public GameObject windowPrefab;

    public Material wallMat;

    public List<Window> windows;

    public WindowLayout layout;

    public GameObject plan1, plan2, plan3, plan4;

    GameObject currentPlan;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layout = new WindowLayout();
        layout.frontWindows = new List<Window>();
        layout.backWindows = new List<Window>();
        layout.leftWindows = new List<Window>();
        layout.rightWindows = new List<Window>();

        Window[] ws = gameObject.GetComponentsInChildren<Window>();

        foreach (Window w in ws)
        {
            w.floor = this;
            windows.Add(w);
            if (w.gameObject.transform.position.x < -35f) layout.frontWindows.Add(w);
            else if (w.gameObject.transform.position.x > 35f) layout.backWindows.Add(w);

            if (w.gameObject.transform.position.z < -54f) layout.leftWindows.Add(w);
            else if (w.gameObject.transform.position.z > 54f) layout.rightWindows.Add(w);
        }

        obj.GetComponent<MeshRenderer>().material.renderQueue = 2002;
    }

    public void SetLayout(int dir, float[] windowsLayout)
    {
        if (dir == 0)
        {
            foreach (Window w in layout.frontWindows) Destroy(w.gameObject);

            layout.frontWindows = new List<Window>();

            foreach (float f in windowsLayout)
            {
                Vector3 pos = new Vector3(-35.61f, 5f, (f - .5f) * floorLength);
                AddWindow(dir, pos);
            }
        }
    }

    public void AddWindow(int dir, Vector3 pos)
    {
        Window window = Instantiate(windowPrefab).GetComponent<Window>();
        window.gameObject.transform.parent = gameObject.transform;
        window.gameObject.transform.localPosition = pos;
        window.gameObject.transform.localRotation = Quaternion.identity;
        window.gameObject.transform.localScale = new Vector3(1, 1, 1);
        window.floor = this;

        windows.Add(window);
        if (dir == 0) layout.frontWindows.Add(window);
        else if (dir == 1) layout.backWindows.Add(window);
        else if (dir == 2) layout.leftWindows.Add(window);
        else if (dir == 3) layout.rightWindows.Add(window);
    }

    public void RemoveWindow(Window w)
    {
        if (layout.frontWindows.Contains(w)) layout.frontWindows.Remove(w);
        if (layout.backWindows.Contains(w)) layout.backWindows.Remove(w);
        if (layout.leftWindows.Contains(w)) layout.leftWindows.Remove(w);
        if (layout.rightWindows.Contains(w)) layout.rightWindows.Remove(w);

        windows.Remove(w);

        Destroy(w.gameObject);
    }

    public Window MoveWindow(Window window, Ray ray)
    {
        if (layout.frontWindows.Contains(window))
        {
            Plane p = new Plane(new Vector3(-1f, 0f, 0f), window.gameObject.transform.position);
            float enter;
            Vector3 proj = new Vector3(0f, 0f, 0f);
            if (p.Raycast(ray, out enter))
            {
                proj = new Vector3(-35.61f, 5f, (ray.GetPoint(enter) - gameObject.transform.position).z);
                foreach (Window w in layout.frontWindows)
                {
                    if (w != window)
                    {
                        if ((proj - w.gameObject.transform.localPosition).magnitude < (w.width + window.width) / 2f)
                            return w;
                    }
                }
            }

            window.gameObject.transform.localPosition = new Vector3(proj.x, proj.y, Mathf.Clamp(proj.z, -55f + window.width / 2f, 55f - window.width / 2f));
            return null;
        }
        else if (layout.backWindows.Contains(window))
        {
        }
        else if (layout.leftWindows.Contains(window))
        {
        }
        else if (layout.rightWindows.Contains(window)) 
        {

        }

        return null;
    }

    public void ScaleWindow(Window window)
    {
        if (layout.frontWindows.Contains(window))
        {
            foreach (Window w in layout.frontWindows)
            {
                if (w != window)
                {
                    if ((window.gameObject.transform.position - w.gameObject.transform.position).magnitude < (w.width + window.width) / 2f)
                    {
                        //MergeWindow(window, w);
                        //break;
                    }
                }
            }
        }
        else if (layout.backWindows.Contains(window))
        {

        }
        else if (layout.leftWindows.Contains(window))
        {

        }
        else if (layout.rightWindows.Contains(window))
        {

        }
    }

    public void MergeWindow(Window a, Window b)
    {
        float width = b.width;
        Vector3 pos = b.gameObject.transform.localPosition;

        List<Window> set = new List<Window>();
        if (layout.frontWindows.Contains(a)) set = layout.frontWindows;
        if (layout.backWindows.Contains(a)) set = layout.backWindows;
        if (layout.leftWindows.Contains(a)) set = layout.leftWindows;
        if (layout.rightWindows.Contains(a)) set = layout.rightWindows;

        windows.Remove(b);
        set.Remove(b);
        Destroy(b.gameObject);

        a.gameObject.transform.localPosition = new Vector3(-35.61f, 5f, Mathf.Clamp((a.gameObject.transform.localPosition + pos).z / 2f, -55f + a.width / 2f, 55f - a.width / 2f));
        a.ScaleWindowLocal(width);
    }

    public void TurnOnWindowsPlan1()
    {
        plan2.SetActive(false);
        plan3.SetActive(false);
        plan4.SetActive(false);
        plan1.SetActive(true);
        
        // Find and deactivate all child objects named "Windows(cloned)"
        foreach (Transform child in transform)
        {
            if (child.name == "Window(Clone)")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void TurnOnWindowsPlan2()
    {
        plan1.SetActive(false);
        plan3.SetActive(false);
        plan4.SetActive(false);
        plan2.SetActive(true);
        foreach (Transform child in transform)
        {
            if (child.name == "Window(Clone)")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void TurnOnWindowsPlan3()
    {
        plan1.SetActive(false);
        plan2.SetActive(false);
        plan4.SetActive(false);
        plan3.SetActive(true);
        foreach (Transform child in transform)
        {
            if (child.name == "Window(Clone)")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void TurnOnWindowsPlan4()
    {
        plan1.SetActive(false);
        plan2.SetActive(false);
        plan3.SetActive(false);
        plan4.SetActive(true);
        foreach (Transform child in transform)
        {
            if (child.name == "Window(Clone)")
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
