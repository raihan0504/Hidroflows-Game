using UnityEngine;

public class Edge : MonoBehaviour
{
    [Header("Connected Nodes")]
    [SerializeField] private Node nodeA;
    [SerializeField] private Node nodeB;

    [Header("Edge Weight")]
    [SerializeField] private int weight = 1;

    [Header("Pipe Visual")]
    [SerializeField] private PipeVisual pipeVisual;

    [Header("Valve")]
    [SerializeField] private Valve valveA;
    [SerializeField] private Valve valveB;

    public Node NodeA => nodeA;
    public Node NodeB => nodeB;
    public int Weight => weight;

    public bool IsSelected { get; set; }
    public bool HasWater { get; set; }

    private void Awake()
    {
        nodeA.AddEdge(this);
        nodeB.AddEdge(this);
    }

    public Node GetOtherNode(Node node)
    {
        if (node == nodeA) return nodeB;
        if (node == nodeB) return nodeA;
        return null;
    }

    public void FlowWater(Node ownerNode)
    {
        if (HasWater) return;

        // ownerNode must be one of the connected nodes
        if (ownerNode != nodeA && ownerNode != nodeB)
        {
            Debug.LogError("FlowWater called with a node that does not belong to this edge.");
            return;
        }

        // Use the explicit owner node to determine flow direction instead of inferring
        Node fromNode = ownerNode;

        valveA.OpenValveAnimation();
        valveB.OpenValveAnimation();

        pipeVisual.SetDirection(fromNode == nodeA ? 0 : 1);

        pipeVisual.FillPipe();

        GetOtherNode(fromNode).Activate();

        HasWater = true;
    }
}
