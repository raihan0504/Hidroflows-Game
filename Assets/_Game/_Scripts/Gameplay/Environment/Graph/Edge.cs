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

    public void FlowWater(float duration)
    {
        Debug.Log("FlowWater Dipanggil");

        if (pipeVisual != null)
        {
            pipeVisual.FIllPipe(duration);
            HasWater = true;
        }
        else
        {
            Debug.LogError("PipeVisual NULL");
        }
    }
}
