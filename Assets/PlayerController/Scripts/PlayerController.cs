using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float airSpeed = 0.4f;
    [SerializeField] private float moveDrag = 4;

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

    private bool cursorIsOn = false;

    private void Awake()
    {
        _input = GetComponent<InputReceiver>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();

        jump = Animator.StringToHash("Jump");
        jumpGrounded = Animator.StringToHash("JumpGrounded");
        movement = Animator.StringToHash("Movement");
    }

    private void Update()
    {
        IsGround();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cursorIsOn)
            {
                cursorIsOn = false;
                _input.CursorOff();
            }
            else
            {
                cursorIsOn = true;
                _input.CursorOn();
            }
        }
    }

    private void FixedUpdate()
    {
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

    IEnumerator SetJump()
    {
        yield return new WaitForSeconds(0.1f);
        isJumping = true;
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
