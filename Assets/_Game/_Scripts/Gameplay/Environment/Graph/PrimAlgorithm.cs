using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrimAlgorithm : MonoBehaviour
{
    [SerializeField] private Graph graph;

    private readonly List<Node> visitedNodes = new();
    private readonly List<Edge> minimumSpanningTree = new();
    private readonly List<Edge> currentCandidates = new();

    private int currentStep;

    public IReadOnlyList<Node> VisitedNodes => visitedNodes;
    public IReadOnlyList<Edge> MinimumSpanningTree => minimumSpanningTree;
    public IReadOnlyList<Edge> CurrentCandidates => currentCandidates;

    public int CurrentStep => currentStep;

    #region Initialize

    public void Initialize(Node startNode)
    {
        Reset();

        startNode.IsVisited = true;
        visitedNodes.Add(startNode);

        UpdateCandidates();
    }

    public void Reset()
    {
        graph.ResetGraph();

        visitedNodes.Clear();
        minimumSpanningTree.Clear();
        currentCandidates.Clear();

        currentStep = 1;
    }

    #endregion

    #region Gameplay

    public PrimResult TrySelectEdge(Edge edge)
    {
        if (IsFinished())
            return PrimResult.Finished;

        if (edge.IsSelected)
            return PrimResult.AlreadySelected;

        if (!currentCandidates.Contains(edge))
            return PrimResult.InvalidCandidate;

        Edge bestEdge = currentCandidates[0];

        if (edge != bestEdge)
            return PrimResult.WrongEdge;

        SelectEdge(edge);

        return PrimResult.Success;
    }

    private void SelectEdge(Edge edge)
    {
        edge.IsSelected = true;

        Node newNode = edge.NodeA.IsVisited
            ? edge.NodeB
            : edge.NodeA;

        newNode.IsVisited = true;

        visitedNodes.Add(newNode);
        minimumSpanningTree.Add(edge);

        currentStep++;

        UpdateCandidates();
    }

    #endregion

    #region Candidate

    private void UpdateCandidates()
    {
        currentCandidates.Clear();

        foreach (Edge edge in graph.Edges)
        {
            bool a = edge.NodeA.IsVisited;
            bool b = edge.NodeB.IsVisited;

            if (a != b && !edge.IsSelected)
            {
                currentCandidates.Add(edge);
            }
        }

        currentCandidates.Sort((x, y) =>
            x.Weight.CompareTo(y.Weight));
    }

    #endregion

    #region Query

    public bool IsFinished()
    {
        return minimumSpanningTree.Count == graph.Nodes.Count - 1;
    }

    public int GetCurrentWeight()
    {
        return minimumSpanningTree.Sum(e => e.Weight);
    }

    #endregion

    #region Utility

    /// <summary>
    /// Digunakan saat Level dimulai untuk menghitung
    /// total bobot MST tanpa mempengaruhi gameplay.
    /// </summary>
    public int CalculateOptimalWeight(Node startNode)
    {
        graph.ResetGraph();

        List<Node> visited = new();
        List<Edge> mst = new();

        startNode.IsVisited = true;
        visited.Add(startNode);

        int total = 0;

        while (mst.Count < graph.Nodes.Count - 1)
        {
            Edge best = null;

            foreach (Edge edge in graph.Edges)
            {
                bool a = edge.NodeA.IsVisited;
                bool b = edge.NodeB.IsVisited;

                if (a == b)
                    continue;

                if (best == null || edge.Weight < best.Weight)
                    best = edge;
            }

            if (best == null)
                break;

            best.NodeA.IsVisited = true;
            best.NodeB.IsVisited = true;

            mst.Add(best);

            total += best.Weight;
        }

        graph.ResetGraph();

        return total;
    }

    #endregion
}

public enum PrimResult
{
    Success,
    WrongEdge,
    AlreadySelected,
    InvalidCandidate,
    Finished
}