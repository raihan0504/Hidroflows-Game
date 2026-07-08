using UnityEngine;

[RequireComponent(typeof(Interact))]
public class Valve : MonoBehaviour
{
    [SerializeField] private Edge edge;
    [SerializeField] private Node ownerNode;

    private Interact interact;

    public Edge Edge => edge;
    public Node OwnerNode => ownerNode;

    private void Awake()
    {
        interact = GetComponent<Interact>();
    }

    private void OnEnable()
    {
        interact.GetInteractEvent.HasInteracted += OpenValve;
    }

    private void OnDisable()
    {
        interact.GetInteractEvent.HasInteracted -= OpenValve;
    }

    private void OpenValve()
    {
        Debug.Log("OpenValve");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance NULL");
            return;
        }

        if (edge == null)
        {
            Debug.LogError("Edge belum di-assign pada Valve");
            return;
        }

        if (!ownerNode.IsActive)
        {
            Debug.Log("Node ini belum dialiri air");
            return;
        }

        if (!edge.NodeA.IsActive && !edge.NodeB.IsActive)
        {
            Debug.Log("Valve belum aktif.");
            return;
        }

        if (edge.HasWater)
        {
            Debug.Log("Edge telah di aliri air");
            return;
        }

        GameManager.Instance.OnValveOpened(this);
    }
}