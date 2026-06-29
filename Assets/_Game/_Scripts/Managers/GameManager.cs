using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PrimAlgorithm primAlgorithm;
    [SerializeField] private WaterTank waterTank;

    [Header("Level")]
    [SerializeField] private Node startNode;

    [Header("Visual")]
    [SerializeField] private float waterFlowDuration = 1f;

    private bool gameFinished;

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

        Debug.Log("Tes");
        Debug.Log(edge);
        Debug.Log(edge.NodeA);
        Debug.Log(edge.NodeB);
        Debug.Log(waterTank);
        Debug.Log(primAlgorithm);

        Debug.Log("----------------------------------");
        Debug.Log($"Valve : {edge.NodeA.NodeID} -> {edge.NodeB.NodeID}");
        Debug.Log($"Weight : {edge.Weight}");

        // Air tidak cukup
        if (!waterTank.CanUseWater(edge.Weight))
        {
            Debug.Log("Air tidak cukup!");

            GameOver();

            return;
        }

        // Air digunakan
        waterTank.UseWater(edge.Weight);

        PrimResult result = primAlgorithm.TrySelectEdge(edge);

        switch (result)
        {
            case PrimResult.Success:

                Debug.Log("Edge BENAR");

                edge.FlowWater(waterFlowDuration);

                if (primAlgorithm.IsFinished())
                {
                    Win();
                }

                break;

            case PrimResult.WrongEdge:

                Debug.Log("Edge SALAH");

                edge.FlowWater(waterFlowDuration);

                break;

            case PrimResult.InvalidCandidate:

                Debug.Log("Edge bukan Candidate");

                break;

            case PrimResult.AlreadySelected:

                Debug.Log("Edge sudah dipilih");

                break;

            case PrimResult.Finished:

                Win();

                break;
        }
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
}