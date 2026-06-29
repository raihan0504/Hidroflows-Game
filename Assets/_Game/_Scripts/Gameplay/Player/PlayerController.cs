using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
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

    private void HandleTouchMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Touch UI");
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Interact interact = hit.collider.GetComponent<Interact>();
                
                if (interact != null)
                {
                    _targetInteract = interact;
                    _targetPosition = interact.transform.position;
                    _hasTarget = true;

                    clickIndicator.position = interact.transform.position;
                    clickIndicator.gameObject.SetActive(true);

                    return;
                }

                _targetPosition = hit.point;
                _hasTarget = true;

                clickIndicator.position = hit.point;
                clickIndicator.gameObject.SetActive(true);
            }
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
            moveSpeed *
            Time.deltaTime);
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
