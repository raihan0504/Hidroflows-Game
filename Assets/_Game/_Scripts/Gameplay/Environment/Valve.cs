using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Interact))]
public class Valve : MonoBehaviour
{
    [SerializeField] private Edge edge;
    [SerializeField] private Node ownerNode;

    private Interact interact;
    private Animator _anim;
    private int openValveHash;

    public Edge Edge => edge;
    public Node OwnerNode => ownerNode;

    private void Awake()
    {
        interact = GetComponent<Interact>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        openValveHash = Animator.StringToHash("ValveTrigger");
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

        if (ownerNode == null)
        {
            Debug.LogError("OwnerNode belum di-assign pada Valve");
            return;
        }

        // Verify the owner node belongs to the assigned edge
        if (ownerNode != edge.NodeA && ownerNode != edge.NodeB)
        {
            Debug.LogError("OwnerNode does not belong to the assigned Edge on this Valve.");
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

        OpenValveAnimation();
        GameManager.Instance.OnValveOpened(this);
    }

    public void OpenValveAnimation()
    {
        _anim.SetTrigger(openValveHash);
    }
}

public enum FlowDirection
{
    NodeAToNodeB,
    NodeBToNodeA
}