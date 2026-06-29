using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> nodes = new();
    [SerializeField] private List<Edge> edges = new();

    public IReadOnlyList<Node> Nodes => nodes;
    public IReadOnlyList<Edge> Edges => edges;

    private void Awake()
    {
        FindGraph();
    }

    private void FindGraph()
    {
        nodes.Clear();
        edges.Clear();

        nodes.AddRange(FindObjectsByType<Node>(FindObjectsSortMode.None));
        edges.AddRange(FindObjectsByType<Edge>(FindObjectsSortMode.None));
    }

    public Node GetNodeByID(int id)
    {
        return nodes.Find(node => node.NodeID == id);
    }

    public void ResetGraph()
    {
        foreach (Node node in nodes)
        {
            node.IsVisited = false;
        }

        foreach (Edge edge in edges)
        {
            edge.IsSelected = false;
            edge.HasWater = false;
        }
    }
}