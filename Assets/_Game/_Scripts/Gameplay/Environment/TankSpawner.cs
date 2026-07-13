using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] private WaterTank waterTank;

    public void MoveTankTo(Node startNode)
    {
        transform.position = startNode.transform.position;
    }
}
