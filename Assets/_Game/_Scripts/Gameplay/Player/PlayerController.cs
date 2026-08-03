using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Component References
    [SerializeField] Transform _cameraTransform;
    [SerializeField] Animator anim;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform clickIndicator;
    private CharacterController _controller;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] LayerMask pipeLayer;
    [SerializeField] float pipeCheckDistance = 1f;

    [Header("Jumping Settings")]
    [SerializeField] float jumpStrength = 5f;

    [Header("Interaction")]
    [SerializeField] float interactDistance = 2f;

    private Vector2 _moveInput;
    private Vector3 _moveDirection;
    private float _verticalVelocity;
    private Vector3 _targetPosition;
    private bool _hasTarget;
    private string currentState;
    private bool _isJumpingPipe;
    private Interact _targetInteract;
    private bool _touchBeganOverUI = false;
    private bool _mousePressedOverUI = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Helper: perform a UI raycast at screen position to detect if over any UI element
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void Update()
    {
        HandleInput();
        CheckPipeAhead();
        HandleMove();
        HandleGravity();
        HandleRotation();
        HandleAnimation();
        HandleTouchMovement();

        HandleInteract();
    }

    private void HandleRotation()
    {
        if (_moveDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(_moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    private void HandleInput()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Keyboard movement
        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _moveDirection = 
                cameraForward * _moveInput.y +
                cameraRight * _moveInput.x;

            _moveDirection.Normalize();

            _hasTarget = false;
            clickIndicator.gameObject.SetActive(false);
        }
        else if (!_hasTarget)
        {
            _moveDirection = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpStrength * -2f * gravity);
        }
    }

    // Draw gizmos to visualize pipe check ray and distance in the editor
    private void OnDrawGizmosSelected()
    {
        // avoid errors when not playing
        Vector3 origin = transform.position + Vector3.up * 0.3f;

        Vector3 dir = Vector3.forward;
        if (Application.isPlaying)
        {
            dir = _moveDirection.sqrMagnitude > 0.01f ? _moveDirection.normalized : transform.forward;
        }
        else
        {
            dir = transform.forward;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + dir * pipeCheckDistance);
        Gizmos.DrawWireSphere(origin + dir * pipeCheckDistance, 0.12f);
    }

    private void HandleTouchMovement()
    {
        // Mobile touch support (Touch & Go). Desktop mouse behavior remains unchanged.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Track whether this touch began over UI so we can ignore the whole touch sequence
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null)
                    _touchBeganOverUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                else
                    _touchBeganOverUI = false;
            }

            // If this touch sequence began over UI, ignore until it ends
            if (_touchBeganOverUI)
            {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    _touchBeganOverUI = false;

                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            // If the touch position is over UI, ignore it to prevent UI clicks from moving the player
            if (IsPointerOverUI(touch.position))
            {
                return;
            }

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            if (hit.collider.CompareTag("Player"))
                return;

            Interact interact = hit.collider.GetComponent<Interact>();

            if (touch.phase == TouchPhase.Began)
            {
                // Interaction priority: if touching an interactable, set it as the interaction target
                // Do NOT trigger interaction immediately; player will auto-walk to it.
                if (interact != null)
                {
                    _targetInteract = interact;
                    _targetPosition = interact.transform.position;
                    _hasTarget = true;

                    clickIndicator.position = interact.transform.position;
                    clickIndicator.gameObject.SetActive(true);
                    return;
                }

                // Tapped on ground: cancel any pending interaction and set destination once
                _targetInteract = null;
                _targetPosition = hit.point;
                _hasTarget = true;

                clickIndicator.position = hit.point;
                clickIndicator.gameObject.SetActive(true);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // While holding or dragging, continuously update destination (follow finger)
                if (interact == null)
                {
                    // dragging on ground: cancel any pending interaction
                    _targetInteract = null;

                    _targetPosition = hit.point;
                    _hasTarget = true;

                    clickIndicator.position = hit.point;
                    if (!clickIndicator.gameObject.activeSelf)
                        clickIndicator.gameObject.SetActive(true);
                }
                else
                {
                    // dragging over an interactable: update target to that interactable so player follows it
                    _targetInteract = interact;
                    _targetPosition = interact.transform.position;
                    _hasTarget = true;

                    clickIndicator.position = interact.transform.position;
                    if (!clickIndicator.gameObject.activeSelf)
                        clickIndicator.gameObject.SetActive(true);
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // On release: keep last destination so character continues to it
            }

            return;
        }

        // Desktop / Mouse behavior - unchanged
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null)
                    _mousePressedOverUI = EventSystem.current.IsPointerOverGameObject();

                if (_mousePressedOverUI)
                    return;
            }

            if (_mousePressedOverUI)
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            Interact interact = hit.collider.GetComponent<Interact>();

            if (interact != null)
            {
                _targetInteract = interact;
                _targetPosition = interact.transform.position;
            }
            else
            {
                _targetInteract = null;
                _targetPosition = hit.point;
            }

            _hasTarget = true;

            clickIndicator.position = _targetPosition;

            if (!clickIndicator.gameObject.activeSelf)
                clickIndicator.gameObject.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mousePressedOverUI = false;
        }
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; // Small negative value to keep the player grounded
        }

        _verticalVelocity += gravity * Time.deltaTime;
        Vector3 gravityMove = new Vector3(0, _verticalVelocity, 0);

        _controller.Move(gravityMove * Time.deltaTime);
    }

    private void HandleMove()
    {
        if (_hasTarget)
        {
            Vector3 direction =
                _targetPosition - transform.position;

            direction.y = 0f;

            // If we have a pending interactable target, check distance and auto-interact when in range
            if (_targetInteract != null)
            {
                float distanceToInteract = Vector3.Distance(transform.position, _targetInteract.transform.position);

                if (distanceToInteract <= interactDistance)
                {
                    // Stop and interact
                    _targetInteract.CallInteract(this);

                    _targetInteract = null;
                    _hasTarget = false;
                    _moveDirection = Vector3.zero;

                    clickIndicator.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    // update movement towards the interactable's current position
                    _targetPosition = _targetInteract.transform.position;
                    direction = _targetPosition - transform.position;
                    direction.y = 0f;
                }
            }

            if (_targetInteract != null)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    _targetInteract.transform.position
                    );

                if (distance <= interactDistance)
                {
                    _targetInteract.CallInteract(this);

                    _targetInteract = null;
                    _hasTarget = false;
                    _moveDirection = Vector3.zero;

                    clickIndicator.gameObject.SetActive(false);
                    return;
                }
            }

            if (direction.magnitude < 0.1f)
            {
                _hasTarget = false;
                _moveDirection = Vector3.zero;

                clickIndicator.gameObject.SetActive(false);
                return;
            }

            _moveDirection = direction.normalized;
        }

        _controller.Move(
            _moveDirection *
            (moveSpeed *
            Time.deltaTime));
    }

    private void CheckPipeAhead()
    {
        if (!_controller.isGrounded)
            return;

        if (_moveDirection.sqrMagnitude < 0.01f)
            return;

        Ray ray = new Ray(
            transform.position + Vector3.up * 0.3f,
            _moveDirection);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            pipeCheckDistance,
            pipeLayer))
        {
            _verticalVelocity = Mathf.Sqrt(jumpStrength * -2f * gravity);
             _isJumpingPipe = true;
        }

        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
            _isJumpingPipe = false;
        }
    }

    #region Animation
    private void HandleAnimation()
    {
        bool isMoving = _moveDirection.sqrMagnitude > 0.01f;

        if (!_controller.isGrounded)
        {
            if (_verticalVelocity > 0)
            {
                ChangeState("Jump", 0);
            }
            else
            {
                ChangeState("Fall", 0);
            }

            return;
        }

        if (isMoving)
        {
            ChangeState("Run", 0);
        }
        else
        {
            ChangeState("Idle");
        }
    }

    private void ChangeState(string newState, float transitionDuration = 0.1f)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        anim.CrossFade(newState, transitionDuration, 0);
    }
    #endregion


    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(
                transform.position + Vector3.up,
                transform.forward
                );

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                Interact interact = hit.collider.GetComponent<Interact>();

                if (interact != null)
                {
                    interact.CallInteract(this);
                }
            }
        }
    }

}
