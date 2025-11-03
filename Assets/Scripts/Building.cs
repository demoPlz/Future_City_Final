using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [System.Serializable]
    public struct FloorWindowLayoutData
    {
        public List<float> frontLayout,
            backLayout,
            leftLayout,
            rightLayout;
    }

    [System.Serializable]
    public struct BuildingWindowLayoutData
    {
        public int slot;
        public List<FloorWindowLayoutData> floorLayouts;
    }

    public float buildingLength,
        buildingWidth;

    public Floor[] floors;
    public GameObject[] floorObjs;
    public GameObject[] plans;
    public int planIdx;
    public int floorNum;

    public GameObject frame;

    public GameObject balconyObject;

    public int floorEditIdx = 0;

    public GameObject rooftop;
    public int additionalFloorNum = 0;
    public GameObject additionalFloorObj;
    public List<GameObject> additionalFloorCopies;

    List<BuildingWindowLayoutData> savedLayoutDatas;

    public int additionalCakeNum = 0;
    public List<GameObject> additionalCakes;

    void Start()
    {
        floorNum = 4;
        SaveBuildingLayout(0);
        //LoadBuildingLayoutFromFile();
        additionalFloorCopies = new List<GameObject>();
    }

    public void SaveBuildingLayout(int slot)
    {
        if (savedLayoutDatas == null)
            savedLayoutDatas = new List<BuildingWindowLayoutData>();

        for (int i = 0; i < savedLayoutDatas.Count; i++)
        {
            BuildingWindowLayoutData layoutData = savedLayoutDatas[i];
            if (layoutData.slot == slot)
            {
                layoutData.floorLayouts = new List<FloorWindowLayoutData>();
                foreach (Floor f in floors)
                {
                    FloorWindowLayoutData floorLayout = new FloorWindowLayoutData();
                    floorLayout.frontLayout = new List<float>();
                    foreach (Window w in f.layout.frontWindows)
                    {
                        floorLayout.frontLayout.Add(
                            w.gameObject.transform.localPosition.z / buildingLength + .5f
                        );
                    }
                    floorLayout.backLayout = new List<float>();
                    foreach (Window w in f.layout.backWindows)
                    {
                        floorLayout.backLayout.Add(
                            w.gameObject.transform.localPosition.z / buildingLength + .5f
                        );
                    }
                    floorLayout.leftLayout = new List<float>();
                    foreach (Window w in f.layout.leftWindows)
                    {
                        floorLayout.leftLayout.Add(
                            w.gameObject.transform.localPosition.x / buildingWidth + .5f
                        );
                    }
                    floorLayout.rightLayout = new List<float>();
                    foreach (Window w in f.layout.rightWindows)
                    {
                        floorLayout.rightLayout.Add(
                            w.gameObject.transform.localPosition.x / buildingWidth + .5f
                        );
                    }
                }

                return;
            }
        }

        BuildingWindowLayoutData newLayout = new BuildingWindowLayoutData();
        newLayout.slot = slot;
        newLayout.floorLayouts = new List<FloorWindowLayoutData>();
        foreach (Floor f in floors)
        {
            FloorWindowLayoutData floorLayout = new FloorWindowLayoutData();
            floorLayout.frontLayout = new List<float>();
            foreach (Window w in f.layout.frontWindows)
            {
                floorLayout.frontLayout.Add(
                    w.gameObject.transform.localPosition.z / buildingLength + .5f
                );
            }
            floorLayout.backLayout = new List<float>();
            foreach (Window w in f.layout.backWindows)
            {
                floorLayout.backLayout.Add(
                    w.gameObject.transform.localPosition.z / buildingLength + .5f
                );
            }
            floorLayout.leftLayout = new List<float>();
            foreach (Window w in f.layout.leftWindows)
            {
                floorLayout.leftLayout.Add(
                    w.gameObject.transform.localPosition.x / buildingWidth + .5f
                );
            }
            floorLayout.rightLayout = new List<float>();
            foreach (Window w in f.layout.rightWindows)
            {
                floorLayout.rightLayout.Add(
                    w.gameObject.transform.localPosition.x / buildingWidth + .5f
                );
            }

            newLayout.floorLayouts.Add(floorLayout);
        }

        savedLayoutDatas.Add(newLayout);
    }

    public void LoadBuildingLayoutFromFile()
    {
        string s = System.IO.File.ReadAllText(Application.dataPath + "/layoutData.json");
        Debug.Log(s);
        BuildingWindowLayoutData layoutData = JsonUtility.FromJson<BuildingWindowLayoutData>(s);
        for (int i = 0; i < layoutData.floorLayouts.Count; i++)
        {
            Debug.Log(i);
            SetWindowsPattern(i, layoutData.floorLayouts[i].frontLayout.ToArray());
        }
    }

    public void LoadBuildingLayout(int slot)
    {
        for (int i = 0; i < savedLayoutDatas.Count; i++)
        {
            BuildingWindowLayoutData layoutData = savedLayoutDatas[i];
            if (layoutData.slot == slot)
            {
                Debug.Log(layoutData.floorLayouts.Count);
                for (int j = 0; j < floors.Length; j++)
                {
                    SetWindowsPattern(j, layoutData.floorLayouts[j].frontLayout.ToArray());
                }

                return;
            }
        }
    }

    public void SwitchUpPlan()
    {
        plans[planIdx].SetActive(false);
        planIdx++;
        if (planIdx == plans.Length)
            planIdx = 0;
        plans[planIdx].SetActive(true);
    }

    public void SwitchDownPlan()
    {
        plans[planIdx].SetActive(false);
        planIdx--;
        if (planIdx == -1)
            planIdx = plans.Length - 1;
        plans[planIdx].SetActive(true);
    }

    public void ClearPlan()
    {
        plans[planIdx].SetActive(false);
        planIdx = 0;
        plans[planIdx].SetActive(true);
    }

    public void SetPlanTwo()
    {
        plans[planIdx].SetActive(false);
        planIdx = 1;
        plans[planIdx].SetActive(true);
    }

    public void SetPlanThree()
    {
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        plans[planIdx].SetActive(false);
        planIdx = 2;
        plans[planIdx].SetActive(true);
    }

    public void SetPlanFour()
    {
        plans[planIdx].SetActive(false);
        planIdx = 3;
        plans[planIdx].SetActive(true);
    }

    public void SetPlanFive()
    {
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        plans[planIdx].SetActive(false);
        planIdx = 4;
        plans[planIdx].SetActive(true);
    }

    public void SetPlanSix()
    {
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        plans[planIdx].SetActive(false);
        planIdx = 5;
        plans[planIdx].SetActive(true);
    }

    public void SwitchPlan(int idx)
    {
        plans[planIdx].SetActive(false);
        planIdx = idx;
        plans[planIdx].SetActive(true);
    }

    public void DanggleFloor()
    {
        int[] floorNumArray = new int[4 * 2 + 1 + 2];
        floorNumArray = new int[] { 4, 4, 3, 2, 1, 0, 1, 2, 3, 4, 4 };
        for (int i = 0; i < floorNumArray[floorEditIdx % floorNumArray.Length]; i++)
        {
            floorObjs[i].SetActive(true);
        }
        for (int i = floorNumArray[floorEditIdx % floorNumArray.Length]; i < 4; i++)
        {
            floorObjs[i].SetActive(false);
        }
        floorEditIdx++;
    }

    public void AddFloor()
    {
        floorNum++;

        for (int i = 0; i < Mathf.Min(4, floorNum); i++)
        {
            floorObjs[i].SetActive(true);
        }
    }

    public void RemoveFloor()
    {
        floorNum--;
        floorNum = Mathf.Max(floorNum, 0);
        if (floorNum < 4)
        {
            for (int i = floorNum; i < 4; i++)
            {
                floorObjs[i].SetActive(false);
            }
        }
    }

    public void SetWindowsPattern(int floor, float[] patterns)
    {
        Floor f = floors[floor];
        foreach (Window w in f.layout.frontWindows)
            Destroy(w.gameObject);

        f.layout.frontWindows = new List<Window>();
        foreach (float p in patterns)
        {
            f.AddWindow(0, new Vector3(-35.61f, 5f, (p - 0.5f) * buildingLength));
        }
    }

    public void CopyWindowsPatterns(int floorToCopy, int floorToModify)
    {
        Floor f = floors[floorToCopy];

        List<float> patterns = new List<float>();

        foreach (Window w in f.layout.frontWindows)
        {
            patterns.Add(w.gameObject.transform.localPosition.z / buildingLength + 0.5f);
        }

        SetWindowsPattern(floorToModify, patterns.ToArray());
    }

    public void ToggleOnBalcony()
    {
        balconyObject.SetActive(!balconyObject.activeSelf);
        frame.SetActive(false);
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        ResetFacade();
    }

    public void AddAdditionalFloor()
    {
        if (additionalCakeNum > 0) ResetAdditionalFloor();

        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();

        additionalFloorNum++;

        foreach (GameObject copy in additionalFloorCopies)
        {
            Destroy(copy);
        }

        additionalFloorCopies.Clear();
        Transform root = additionalFloorObj.transform;
        for (int i = 0; i < additionalFloorNum; i++)
        {
            GameObject copy = Instantiate(additionalFloorObj);
            copy.transform.parent = root.parent;
            copy.transform.localPosition = new Vector3(0, i * 10.5f, 0);
            copy.transform.localRotation = Quaternion.identity;
            copy.transform.localScale = new Vector3(1, 1, 1);
            copy.SetActive(true);
            additionalFloorCopies.Add(copy);
        }

        rooftop.transform.localPosition = new Vector3(rooftop.transform.localPosition.x, 10.1f + (additionalFloorNum - 1) * 10.5f, rooftop.transform.localPosition.z);
    }

    public void RemoveAdditionalFloor()
    {
        if (additionalFloorNum == 0) return;
        additionalFloorNum--;
        GameObject copy = additionalFloorCopies[additionalFloorCopies.Count - 1];
        Destroy(copy);
        additionalFloorCopies.RemoveAt(additionalFloorNum);
        rooftop.transform.localPosition = new Vector3(rooftop.transform.localPosition.x, 10.1f + (additionalFloorNum - 1) * 10.5f, rooftop.transform.localPosition.z);
    }

    public void AddAdditionalCake()
    {
        if (additionalFloorNum > 0) ResetAdditionalFloor();

        RemoveFloor();
        RemoveFloor();
        RemoveFloor();
        RemoveFloor();

        additionalCakeNum++;
        additionalCakeNum = Mathf.Min(additionalCakeNum, 5);
        for (int i = 0; i < additionalCakeNum; i++)
        {
            additionalCakes[i].SetActive(true);
        }

        rooftop.SetActive(false);
    }

    public void RemoveAdditionalCake()
    {
        if (additionalCakeNum == 0) return;

        additionalCakeNum--;
        additionalCakes[additionalCakeNum].SetActive(false);
        if (additionalCakeNum == 0)
        {
            rooftop.SetActive(true);
        }
    }

    public void ResetAdditionalFloor()
    {
        additionalCakeNum = 0;
        foreach (GameObject cake in additionalCakes)
        {
            cake.SetActive(false);
        }

        additionalFloorNum = 0;
        foreach (GameObject copy in additionalFloorCopies)
        {
            Destroy(copy);
        }

        additionalFloorCopies.Clear();
        rooftop.transform.localPosition = new Vector3(rooftop.transform.localPosition.x, 0f, rooftop.transform.localPosition.z);
        rooftop.SetActive(true);
    }

    public void ResetFacade()
    {
        foreach (GameObject plan in plans)
        {
            plan.SetActive(false);
        }
    }
}
