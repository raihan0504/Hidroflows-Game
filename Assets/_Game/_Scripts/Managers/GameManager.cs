using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PrimAlgorithm primAlgorithm;
    [SerializeField] private WaterTank waterTank;
    [SerializeField] private Graph graph;

    [Header("Level")]
    [SerializeField] private Node startNode;

    [Header("Visual")]
    [SerializeField] private float pipeLerpSpeed = 2f;

    [Header("Tank")]
    [SerializeField] private TankSpawner tanksSpawner;
    [SerializeField] private TankSpawnPoint[] spawnPoint;

    private bool gameFinished;

    public float PipeLerpSpeed => pipeLerpSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        gameFinished = false;

        startNode = GetRandomStartNode();

        Debug.Log($"Start Node Random : {startNode.NodeID}");

        PositionTank(startNode);

        int optimalWeight = primAlgorithm.CalculateOptimalWeight(startNode);

        waterTank.Initialize(optimalWeight);
        primAlgorithm.Initialize(startNode);

        Debug.Log("==================================");
        Debug.Log("LEVEL DIMULAI");
        Debug.Log($"Start Node : {startNode.NodeID}");
        Debug.Log($"Total Air : {optimalWeight}");
        Debug.Log("==================================");
    }

    public void OnValveOpened(Valve valve)
    {
        if (gameFinished)
            return;

        Edge edge = valve.Edge;

        // Exploration rules: allow opening if edge hasn't water and owner node is active
        if (edge.HasWater)
        {
            Debug.Log("Edge telah di aliri air");
            return;
        }

        if (valve.OwnerNode == null)
        {
            Debug.LogError("Valve OwnerNode is null");
            return;
        }

        if (!valve.OwnerNode.IsActive)
        {
            Debug.Log("Node ini belum dialiri air");
            return;
        }

        Debug.Log("----------------------------------");
        Debug.Log($"Valve : {edge.NodeA.NodeID} -> {edge.NodeB.NodeID}");
        Debug.Log($"Weight : {edge.Weight}");
        Debug.Log($"[GameManager] Edge.NodeA(ID:{edge.NodeA.NodeID}).IsVisited = {edge.NodeA.IsVisited}");
        Debug.Log($"[GameManager] Edge.NodeB(ID:{edge.NodeB.NodeID}).IsVisited = {edge.NodeB.IsVisited}");

        // Evaluate selection for Prim scoring, but do not block gameplay
        PrimResult result = primAlgorithm.EvaluateSelection(edge);

        switch (result)
        {
            case PrimResult.Success:
                Debug.Log("Prim: Correct selection");
                break;
            case PrimResult.WrongEdge:
                Debug.Log("Prim: Wrong selection (but exploration allowed)");
                break;
            case PrimResult.InvalidCandidate:
                Debug.Log("Prim: InvalidCandidate (exploration allowed)");
                break;
            case PrimResult.AlreadySelected:
                Debug.Log("Prim: AlreadySelected");
                // If already selected, do not re-flow or consume water
                return;
            case PrimResult.Finished:
                Debug.Log("Prim: Finished");
                break;
        }

        // Common exploration behavior: consume water and flow if enough water
        if (!waterTank.CanUseWater(edge.Weight))
        {
            Debug.Log("Air tidak cukup!");
            GameOver();
            return;
        }

        waterTank.UseWater(edge.Weight);

        edge.FlowWater(valve.OwnerNode);

        // If Prim finished as a result of EvaluateSelection, check win
        if (primAlgorithm.IsFinished())
            Win();
    }

    private void Win()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        Debug.Log("==================================");
        Debug.Log("PLAYER MENANG");
        Debug.Log($"Total Bobot : {primAlgorithm.GetCurrentWeight()}");
        Debug.Log("==================================");
    }

    private void GameOver()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        Debug.Log("==================================");
        Debug.Log("GAME OVER");
        Debug.Log("Air Habis");
        Debug.Log("==================================");
    }

    private Node GetRandomStartNode()
    {
        int randomIndex = Random.Range(0, graph.Nodes.Count);
        return graph.Nodes[randomIndex];
    }

    private void PositionTank (Node startNode)
    {
        foreach (TankSpawnPoint point in spawnPoint)
        {
            if (point.Node == startNode)
            {
                tanksSpawner.transform.SetPositionAndRotation(
                    point.transform.position,
                    point.transform.rotation);

                return;
            }
        }
    }
}