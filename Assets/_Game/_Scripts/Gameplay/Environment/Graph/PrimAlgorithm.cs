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
        if (startNode == null)
        {
            Debug.LogError("PrimAlgorithm.Initialize called with null startNode");
            return;
        }

        Reset();

        startNode.IsVisited = true;
        startNode.Activate();

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

        // Legacy method: keep behavior but do not change gameplay flow here.
        // This method still evaluates and, if correct, advances Prim algorithm state.
        bool isInCandidates = currentCandidates.Contains(edge);

        if (!isInCandidates)
        {
            return PrimResult.InvalidCandidate;
        }

        Edge bestEdge = currentCandidates[0];

        if (edge != bestEdge)
            return PrimResult.WrongEdge;

        SelectEdge(edge);

        return PrimResult.Success;
    }

    /// <summary>
    /// Evaluate a player selection for scoring. This does not block gameplay.
    /// If the selected edge is the current best Prim candidate, this will advance Prim state.
    /// Otherwise it will return Wrong or InvalidCandidate but will NOT prevent gameplay actions.
    /// </summary>
    public PrimResult EvaluateSelection(Edge edge)
    {
        return TrySelectEdge(edge);
    }

    private void SelectEdge(Edge edge)
    {
        Debug.Log("===== SELECT EDGE =====");
        Debug.Log($"Edge : {edge.NodeA.NodeID}-{edge.NodeB.NodeID}");

        edge.IsSelected = true;

        Node newNode = edge.NodeA.IsVisited
            ? edge.NodeB
            : edge.NodeA;

        Debug.Log($"Node baru = {newNode.NodeID}");

        newNode.IsVisited = true;
        newNode.Activate();

        Debug.Log($"Node {newNode.NodeID} IsVisited = {newNode.IsVisited}");

        visitedNodes.Add(newNode);
        minimumSpanningTree.Add(edge);

        UpdateCandidates();

        Debug.Log("Visited:");
        foreach (Node node in visitedNodes)
            Debug.Log(node.NodeID);

        Debug.Log("Candidates:");
        foreach (Edge e in currentCandidates)
            Debug.Log($"{e.NodeA.NodeID}-{e.NodeB.NodeID}");
    }

    #endregion

    #region Candidate

    private void UpdateCandidates()
    {
        currentCandidates.Clear();

        // Validate that graph still contains all edges (can catch issues with late-loaded edges)
        int graphEdgeCount = graph.Edges.Count;
        if (graphEdgeCount == 0)
        {
            Debug.LogError("[UpdateCandidates] Graph contains NO edges! Graph discovery failed or edges not loaded.");
            return;
        }

        foreach (Edge edge in graph.Edges)
        {
            // Defensive null checks
            if (edge == null)
            {
                Debug.LogWarning("[UpdateCandidates] Null edge found in graph.Edges");
                continue;
            }

            if (edge.NodeA == null || edge.NodeB == null)
            {
                Debug.LogWarning($"[UpdateCandidates] Edge has null nodes: NodeA={edge.NodeA}, NodeB={edge.NodeB}");
                continue;
            }

            bool a = edge.NodeA.IsVisited;
            bool b = edge.NodeB.IsVisited;

            if (a != b && !edge.IsSelected)
            {
                currentCandidates.Add(edge);
            }
        }

        // Primary sort by weight, secondary deterministic tiebreaker by node IDs
        currentCandidates.Sort((x, y) =>
        {
            int cmp = x.Weight.CompareTo(y.Weight);
            if (cmp != 0) return cmp;

            // use smaller (min) node id pair as tiebreaker to keep sort stable/deterministic
            int xMin = Mathf.Min(x.NodeA.NodeID, x.NodeB.NodeID);
            int yMin = Mathf.Min(y.NodeA.NodeID, y.NodeB.NodeID);
            cmp = xMin.CompareTo(yMin);
            if (cmp != 0) return cmp;

            int xMax = Mathf.Max(x.NodeA.NodeID, x.NodeB.NodeID);
            int yMax = Mathf.Max(y.NodeA.NodeID, y.NodeB.NodeID);
            return xMax.CompareTo(yMax);
        });

        // DEBUG: Log update candidates
        string candidateList = string.Join(", ", currentCandidates.Select(e => $"{e.NodeA.NodeID}-{e.NodeB.NodeID}(w:{e.Weight})"));
        Debug.Log($"[UpdateCandidates] Graph has {graphEdgeCount} edges total. Candidates after filtering: [{candidateList}]");
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

            // Mark only the not-yet-visited node (safer than setting both)
            if (!best.NodeA.IsVisited)
                best.NodeA.IsVisited = true;
            else if (!best.NodeB.IsVisited)
                best.NodeB.IsVisited = true;

            mst.Add(best);

            total += best.Weight;
        }

        graph.ResetGraph();

        return total;
    }

    /// <summary>
    /// Debug helper: Print current game state
    /// </summary>
    private void PrintGameState(string marker)
    {
        string visited = string.Join(", ", visitedNodes.Select(n => n.NodeID));
        string candidates = string.Join(", ", currentCandidates.Select(e => $"{e.NodeA.NodeID}-{e.NodeB.NodeID}(w:{e.Weight})"));
        
        Debug.Log($"=== {marker} ===");
        Debug.Log($"Visited Nodes: [{visited}]");
        Debug.Log($"Active Nodes: {string.Join(", ", graph.Nodes.Where(n => n.IsActive).Select(n => n.NodeID))}");
        Debug.Log($"Current Candidates: [{candidates}]");
        Debug.Log($"Selected Edges (MST): {minimumSpanningTree.Count}");
        Debug.Log($"===========================");
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