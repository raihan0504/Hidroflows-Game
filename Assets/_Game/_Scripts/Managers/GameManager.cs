using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PrimAlgorithm primAlgorithm;
    [SerializeField] private WaterTank waterTank;
    [SerializeField] private Graph graph;

    [Header("Level")]
    [SerializeField] LevelManager levelManager;
    [SerializeField] private Node startNode;

    [Header("Level Timer")]
    [SerializeField] private float levelTime = 180f;

    private float currentTime;

    [Header("Visual")]
    [SerializeField] private float pipeLerpSpeed = 2f;

    [Header("Tank")]
    [SerializeField] private TankSpawner tanksSpawner;
    [SerializeField] private TankSpawnPoint[] spawnPoint;

    private bool gameFinished;

    public GameState CurrentState { get; private set; } = GameState.Loading;

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
        // Loading -> initialize -> Playing
        ChangeState(GameState.Loading);

        gameFinished = false;

        startNode = GetRandomStartNode();

        Debug.Log($"Start Node Random : {startNode.NodeID}");

        PositionTank(startNode);

        int optimalWeight = primAlgorithm.CalculateOptimalWeight(startNode);

        waterTank.Initialize(optimalWeight);
        primAlgorithm.Initialize(startNode);

        // GameManager will handle win/lose ordering itself after applying an edge.
        // Do not subscribe to WaterTank.OnWaterEmpty here to avoid race where OnWaterEmpty
        // fires before we can check Prim win condition. The GameManager will check
        // waterTank.IsEmpty() after flow and prim evaluation.

        Debug.Log("==================================");
        Debug.Log("LEVEL DIMULAI");
        Debug.Log($"Start Node : {startNode.NodeID}");
        Debug.Log($"Total Air : {optimalWeight}");
        Debug.Log("==================================");

        // initialize timer
        currentTime = levelTime;

        ChangeState(GameState.Playing);
    }

    private void Update()
    {
        // Only count down while playing and the game hasn't finished
        if (CurrentState == GameState.Playing && !gameFinished)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f) currentTime = 0f;

            if (currentTime <= 0f)
            {
                // time expired -> game over if not already finished
                if (!gameFinished)
                    GameOver();
            }
        }
    }

    public float CurrentTime => currentTime;

    public void OnValveOpened(Valve valve)
    {
        // Only allow valve interaction while playing
        if (CurrentState != GameState.Playing)
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

        // After applying the flow, prioritize win check before checking empty water.
        if (primAlgorithm.IsFinished())
        {
            // Player completed MST ? win even if water is exactly zero
            Win();
            return;
        }

        // If MST not complete and water is empty after consumption, it's game over
        if (waterTank.IsEmpty())
        {
            GameOver();
            return;
        }
    }

    private void Win()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        levelManager.CompleteLevel();
        ChangeState(GameState.Win);

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

        ChangeState(GameState.Lose);

        Debug.Log("==================================");
        Debug.Log("GAME OVER");
        Debug.Log("Air Habis");
        Debug.Log("==================================");
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"GameState changed to: {newState}");
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