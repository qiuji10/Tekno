using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class PlayerController : MonoBehaviour, IKnockable
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    public static bool allowedInput = true;
    public static bool allowedAction { get; set; } = true;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float airSpeed = 0.4f;
    [SerializeField] private float moveDrag = 4;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private float fallMultiplier = 6.2f;
    [SerializeField] private float lowJumpMultiplier = 1.7f;
    private bool isJumping;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundedRadius = 0.28f;
    [SerializeField] private float groundedOffset = -0.14f;
    [SerializeField] private bool isGround;

    [Header("Animation Blend")]
    [SerializeField] private float animMoveSpeed = 0.8f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    private float velocity;

    [Header("Input Action Reference")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference jumpAction;

    private Rigidbody _rb;
    private Animator _anim;
    public Animator Anim { get { return _anim; } set { _anim = value; } }

    private int movement, jump, jumpGrounded;

    private void Awake()
    {
        allowedInput = allowedAction = true;

        if (DualShockGamepad.current != null) DualShockGamepad.current.SetLightBarColor(Color.cyan * 0.5f);
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();

        jump = Animator.StringToHash("Jump");
        jumpGrounded = Animator.StringToHash("JumpGrounded");
        movement = Animator.StringToHash("Movement");
    }

    private void OnEnable()
    {
        jumpAction.action.performed += Jump;
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track track)
    {
        switch (track.genre)
        {
            case Genre.House:
                moveSpeed = 10.8f;
                animMoveSpeed = 0.7f;
                if (DualShockGamepad.current != null) DualShockGamepad.current.SetLightBarColor(Color.yellow * 0.5f);
                break;
            case Genre.Techno:
                moveSpeed = 10.9f;
                animMoveSpeed = 0.7875f;
                if (DualShockGamepad.current != null) DualShockGamepad.current.SetLightBarColor(Color.cyan * 0.5f);
                break;
            case Genre.Electronic:
                moveSpeed = 11;
                animMoveSpeed = 0.9f;
                if (DualShockGamepad.current != null) DualShockGamepad.current.SetLightBarColor(Color.green * 0.5f);
                break;
        }
    }

    private void Update()
    {
        IsGround();
    }

    private void FixedUpdate()
    {
        if (_rb.isKinematic)
            velocity = 0;

        _anim.SetFloat(movement, velocity);

        if (!allowedInput) return;

        JumpVelocitySmoother();
        SpeedLimiter();
        Movement();
    }

    private void IsGround()
    {
        //isGround = Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, groundLayer);

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGround = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if (isGround)
        {
            if (isJumping)
            {
                isJumping = false;
                _anim.ResetTrigger(jump);
                _anim.SetTrigger(jumpGrounded);
            }
            _rb.drag = moveDrag;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    private void Movement()
    {
        Vector2 move = movementAction.action.ReadValue<Vector2>();
        Vector3 _moveDir = orientation.forward * move.y + orientation.right * move.x;

        if (_moveDir == Vector3.zero)
        {
            _rb.angularVelocity = Vector3.zero;
            // velocity for animation blend
            if (velocity > 0)
            {
                velocity -= Time.fixedDeltaTime * deceleration;
            }
            else
            {
                velocity = 0;
            }
        }
        else
        {
            if (isGround)
            {
                _rb.AddForce(_moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

                // velocity for animation blend
                if (velocity < animMoveSpeed)
                {
                    velocity += Time.fixedDeltaTime * acceleration;
                }
                else
                {
                    velocity = 1 * animMoveSpeed;
                }
            }
            else
            {
                _rb.AddForce(_moveDir.normalized * moveSpeed * 10f * airSpeed, ForceMode.Force);
            }
        }
    }

    private void SpeedLimiter()
    {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
        }
    }

    private void JumpVelocitySmoother()
    {
        if (_rb.velocity.y < 0.5f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0.5f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!allowedAction || !allowedInput) return;

        if (isGround && !isJumping)
        {
            StartCoroutine(SetJump());
            _anim.ResetTrigger(jumpGrounded);
            _anim.SetTrigger(jump);
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private IEnumerator SetJump()
    {
        yield return new WaitForSeconds(0.1f);
        isJumping = true;
    }

    public void Knock(Vector3 direction, float power)
    {
        StartCoroutine(KnockBack(direction, power));
    }

    private IEnumerator KnockBack(Vector3 direction, float power)
    {
        allowedInput = false;
        _rb.AddForce(direction * power, ForceMode.Impulse);
        yield return new WaitForSeconds(1.1f);
        allowedInput = true;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }
}
