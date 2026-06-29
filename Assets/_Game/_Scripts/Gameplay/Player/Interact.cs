using UnityEngine;

public class Interact : MonoBehaviour
{
    InteractEvent _interact = new InteractEvent();
    PlayerController _player;

    public InteractEvent GetInteractEvent => _interact;

    public PlayerController GetPlayer
    {
        get
        {
            return _player;
        }
    }

    public void CallInteract(PlayerController interactPlayer)
    {
        _player = interactPlayer;
        _interact.CallInteractEvent();
    }
}

public class InteractEvent
{
    public delegate void InteractHandler();

    public event InteractHandler HasInteracted;

    public void CallInteractEvent() => HasInteracted?.Invoke();
}