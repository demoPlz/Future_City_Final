using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneToAlign : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;

    [SerializeField]
    Material m_DefaultMat;

    [SerializeField]
    Material m_SelectedMat;

    public int selectingPointer { get; private set; } = MyTouch.k_Deselected;

    string m_SceneName;

    public bool hide = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_SceneName = SceneManager.GetActiveScene().name;
    }

    public void SetSelected(int pointer)
    {

        if(hide)
        {
            return;
        }

        var isSelected = pointer != MyTouch.k_Deselected;
        selectingPointer = pointer;
        m_MeshRenderer.material = isSelected ? m_SelectedMat : m_DefaultMat;
        Debug.Log($"Plane selectingPointer {pointer}, isSelected {isSelected} selectingPointer {selectingPointer}");
    }
}
