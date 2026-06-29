using UnityEngine;

[RequireComponent(typeof(Interact))]
public class Valve : MonoBehaviour
{
    [SerializeField] private Edge edge;

    private Interact interact;

    public Edge Edge => edge;

    private void Awake()
    {
        interact = GetComponent<Interact>();
    }

    private void OnEnable()
    {
        interact.GetInteractEvent.HasInteracted += OpenValve;
    }

    private void OnDisable()
    {
        interact.GetInteractEvent.HasInteracted -= OpenValve;
    }

    private void OpenValve()
    {
        Debug.Log("OpenValve");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance NULL");
            return;
        }

        if (edge == null)
        {
            Debug.LogError("Edge belum di-assign pada Valve");
            return;
        }

        GameManager.Instance.OnValveOpened(this);
    }
}