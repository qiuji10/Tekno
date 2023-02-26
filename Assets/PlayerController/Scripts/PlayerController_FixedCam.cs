using System.Collections;
using UnityEngine;

public class PlayerController_FixedCam : MonoBehaviour, IKnockable
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    private Transform _playerObj;
    public bool allowedInput = true;
    public bool allowedAction { get; set; } = true;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float airSpeed = 0.4f;
    [SerializeField] private float moveDrag = 4;
    [SerializeField] private float rotationSpeed = 7f;

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
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    private float velocity;

    private InputReceiver _input;
    private Rigidbody _rb;
    private Animator _anim;

    private int movement, jump, jumpGrounded;

    private void Awake()
    {
        _input = GetComponent<InputReceiver>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _playerObj = transform.GetChild(0);

        jump = Animator.StringToHash("Jump");
        jumpGrounded = Animator.StringToHash("JumpGrounded");
        movement = Animator.StringToHash("Movement");
    }

    private void Update()
    {
        if (!allowedInput) return;

        //Rotation();
        IsGround();

        if (!allowedAction) return;
        if (_input.attack)
            _input.attack = false;
        if (_input.heavyAttack)
            _input.heavyAttack = false;
        if (_input.square)
            _input.square = false;
    }

    private void FixedUpdate()
    {
        if (!allowedInput) return;

        Jump();
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
        Vector3 _moveDir = orientation.forward * _input.move.y + orientation.right * _input.move.x;

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
                if (velocity < 1f)
                {
                    velocity += Time.fixedDeltaTime * acceleration;
                }
                else
                {
                    velocity = 1;
                }
            }
            else
            {
                _rb.AddForce(_moveDir.normalized * moveSpeed * 10f * airSpeed, ForceMode.Force);
            }
        }

        _anim.SetFloat(movement, velocity);
    }

    private void Rotation()
    {
        Vector3 inputDir = new Vector3(_input.move.x, 0, _input.move.y);

        if (inputDir != Vector3.zero)
            _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
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

    private void Jump()
    {
        if (_rb.velocity.y < 0.5f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0.5f)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (!allowedAction) return;

        if (_input.jump)
        {
            _input.jump = false;

            if (isGround && !isJumping)
            {
                StartCoroutine(SetJump());
                _anim.ResetTrigger(jumpGrounded);
                _anim.SetTrigger(jump);
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
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
