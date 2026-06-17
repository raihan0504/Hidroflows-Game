using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Interactions;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

    private GameInput _input;

    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnHold;
    public event Action OnRelease;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);

        _input = new GameInput();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Tap.performed += HandleTap;
        _input.Player.Tap.canceled += HandleRelease;
    }

    private void OnDisable()
    {
        _input.Player.Tap.performed -= HandleTap;
        _input.Player.Tap.canceled -= HandleRelease;

        _input.Disable();
    }

    private void HandleTap(InputAction.CallbackContext context)
    {
        Vector2 pos = _input.Player.Position.ReadValue<Vector2>();

        if (context.interaction is TapInteraction)
        {
            OnTap?.Invoke(pos);
        }
        else if (context.interaction is HoldInteraction)
        {
            OnHold?.Invoke(pos);
        }
    }

    private void HandleRelease(InputAction.CallbackContext context)
    {
        OnRelease?.Invoke();
    }
}
