using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node")]
    [SerializeField] private int nodeID;

    [Header("Plant")]
    [SerializeField] private Sawi sawi;

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

        if (sawi != null)
            sawi.PlayGrowAnimation();
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