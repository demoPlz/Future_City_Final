using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MySelectable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        // [SerializeField]
        MeshRenderer m_MeshRenderer;

        // [SerializeField]
        Material m_DefaultMat;

        [SerializeField]
        Material m_SelectedMat;

        Rigidbody m_RigidBody;
        BoxCollider m_BoxCollider;

        // bool m_IsCollided = false;

        public int selectingPointer { get; private set; } = MyTouch.k_Deselected;

        void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_BoxCollider = GetComponent<BoxCollider>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_DefaultMat = m_MeshRenderer.material;
        }
    
        public void SetSelected(int pointer)
        {
            var isSelected = pointer != MyTouch.k_Deselected;
            selectingPointer = pointer;
            m_MeshRenderer.material = isSelected ? m_SelectedMat : m_DefaultMat;
            // @Carton: should not change the isKinematic property here
            // m_RigidBody.isKinematic = isSelected;
            Debug.Log($"Piece selectingPointer {pointer}, isSelected {isSelected}");
        }
}
