using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private int nodeID;
    private readonly List<Edge> connectedEdges = new();
    public bool IsVisited { get; set; }

    public int NodeID => nodeID;
    public IReadOnlyList<Edge> ConnectedEdges => connectedEdges;

    public void AddEdge(Edge edge)
    {
        if (!connectedEdges.Contains(edge)) 
            connectedEdges.Add(edge);
    }
}
