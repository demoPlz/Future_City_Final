using UnityEngine;

public class Window : MonoBehaviour
{
    public GameObject upperFrame,
        lowerFrame,
        leftFrame,
        rightFrame;
    public GameObject glass;
    public float width;
    public Floor floor;

    public Material normalMat,
        selectedMat;

    void Start()
    {
        width = glass.transform.localScale.z;
    }

    public void TurnOffCollider()
    {
        glass.GetComponent<BoxCollider>().enabled = false;
    }

    public void SetOnSelectedVisual(bool selected)
    {
        glass.GetComponent<MeshRenderer>().material = selected ? selectedMat : normalMat;
    }

    public Window MoveWindow(Ray ray)
    {
        return floor.MoveWindow(this, ray);
    }

    public void ScaleWindow(float d)
    {
        ScaleWindowLocal(d);
        floor.ScaleWindow(this);
    }

    public void ScaleWindowLocal(float d)
    {
        width += d;

        if (width > 0f)
        {
            leftFrame.transform.localPosition = new Vector3(
                leftFrame.transform.localPosition.x,
                leftFrame.transform.localPosition.y,
                width / 2f
            );
            rightFrame.transform.localPosition = new Vector3(
                rightFrame.transform.localPosition.x,
                rightFrame.transform.localPosition.y,
                -width / 2f
            );
            upperFrame.transform.localScale = new Vector3(
                upperFrame.transform.localScale.x,
                upperFrame.transform.localScale.y,
                width
            );
            lowerFrame.transform.localScale = new Vector3(
                lowerFrame.transform.localScale.x,
                lowerFrame.transform.localScale.y,
                width
            );

            glass.transform.localScale = new Vector3(
                glass.transform.localScale.x,
                glass.transform.localScale.y,
                width
            );
        }
    }
}
