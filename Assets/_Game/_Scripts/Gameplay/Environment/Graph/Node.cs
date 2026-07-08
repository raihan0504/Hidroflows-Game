using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private int nodeID;

    private readonly List<Edge> connectedEdges = new();

    public int NodeID => nodeID;
    public IReadOnlyList<Edge> ConnectedEdges => connectedEdges;

    // Untuk Algoritma Prim
    public bool IsVisited { get; set; }

    // Untuk Gameplay (air sudah mencapai node)
    public bool IsActive { get; private set; }

    public void AddEdge(Edge edge)
    {
        if (!connectedEdges.Contains(edge))
            connectedEdges.Add(edge);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ResetNode()
    {
        IsVisited = false;
        IsActive = false;
    }
}