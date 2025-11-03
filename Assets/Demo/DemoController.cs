using UnityEngine;

public class DemoController : MonoBehaviour
{
    public InteractionController interaction;

    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        interaction.UpdateRaycastResult(ray);
        interaction.UpdateInputValue(Input.mouseScrollDelta.y, .2f);
        if (Input.GetKeyDown("space"))
            interaction.UpdateInputButton();

        if (Input.GetKeyDown("a"))
        {
            interaction.SwitchMode(false);
        }

        if (Input.GetKeyDown("d"))
        {
            interaction.SwitchMode(true);
        }

        if (Input.GetKeyDown("c"))
        {
            interaction.Checkerboard();
        }

        if (Input.GetKeyDown("q"))
        {
            interaction.CopyPatterns();
        }

        if (Input.GetKeyDown("l"))
        {
            interaction.RestoreInitLayout();
        }

        if (Input.GetMouseButtonDown(0))
        {
            interaction.OnPress();
        }

        if (Input.GetMouseButtonUp(0))
        {
            interaction.OnRelease();
        }
    }
}
