using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookAbility : MonoBehaviour
{
    [Header("Hook Settings")]
    [SerializeField] private float hookRange = 10;
    [SerializeField] private float forwardThrustForce;
    [SerializeField] private float throwForwardBurst = 50f;
    [SerializeField] private float throwDownBurst = 25f;

    [SerializeField] private float anchor = -4;
    [SerializeField] private float angle = 45;

    [SerializeField] private float offsetBeatTime = 0.3f;

    [SerializeField] private Vector3 detectOffset;
    [SerializeField] private Vector3 handPoint;
    [SerializeField] private Vector3 grabOffset;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private InputActionReference hookAction;
    [SerializeField] private InputActionReference movementAction;

    private PlayerController _playerController;
    private Rigidbody _rb;
    private HingeJoint _joint;

    public bool isHooking;
    private float oriMoveSpeed;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        hookAction.action.performed += Hook;
        hookAction.action.canceled += Hook;
    }

    private void OnDisable()
    {
        hookAction.action.performed -= Hook;
        hookAction.action.canceled -= Hook;
    }

    private void Update()
    {
        if (isHooking) 
        {
            HookMovement();
        }
    }

    private void Hook(InputAction.CallbackContext context)
    {
        if (isHooking) 
        {
            if (Time.time > TempoManager._lastBeatTime - offsetBeatTime && Time.time < TempoManager._lastBeatTime + offsetBeatTime)
            {
                Debug.Log($"<color=magenta>Release On Beat");
                Vector3 dir = orientation.TransformDirection(orientation.forward);
                Vector3 realDir = new Vector3(dir.x, 0f, dir.z); 
                _rb.AddForce(realDir * throwForwardBurst, ForceMode.Impulse);
            }
            else
            {
                Vector3 dir = -orientation.up;
                _rb.AddForce(dir * throwDownBurst, ForceMode.Impulse);
            }

            isHooking = false;
            _playerController.transform.eulerAngles = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _playerController.MoveSpeed = oriMoveSpeed;
            _playerController.Anim.SetTrigger("EndSwing");
            lineRenderer.positionCount = 0;
            Destroy(_joint);
            _joint = null;
        }
        else
        {
            //_playerController.enabled = false;
            
            Collider[] collideData = Physics.OverlapSphere(transform.position + detectOffset, hookRange);

            foreach (Collider collide in collideData) 
            { 
                if (collide.CompareTag("Hook") && collide.TryGetComponent(out Rigidbody rb))
                {
                    _joint = gameObject.AddComponent<HingeJoint>();
                    _joint.axis = new Vector3(collide.transform.right.x, collide.transform.right.y, collide.transform.right.z);
                    

                    if (collide.transform.position.y >= transform.position.y)
                    {
                        _joint.anchor = new Vector3(0, 6f, 0f);
                    }
                    else
                    {
                        _joint.anchor = new Vector3(0, -10f, 0f);
                    }
                    

                    JointLimits limits = new JointLimits();
                    limits.min = -angle;
                    limits.max = angle;
                    _joint.limits = limits;
                    _joint.useLimits = true;

                    _joint.autoConfigureConnectedAnchor = true;
                    _joint.connectedBody = rb;

                    //JointSpring springJoint = new JointSpring();
                    //springJoint.damper = damper;
                    //springJoint.spring = spring;
                    //_joint.spring = springJoint;
                    //_joint.useSpring = true;
                    _joint.massScale = 4.5f;

                    _rb.constraints = RigidbodyConstraints.None;

                    oriMoveSpeed = _playerController.MoveSpeed;
                    _playerController.MoveSpeed *= 1.5f;
                    _playerController.transform.rotation = transform.rotation;
                    _playerController.Anim.SetTrigger("StartSwing");

                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, rb.position);

                    isHooking = true;

                    break;
                }
            }
        }
    }

    private void HookMovement()
    {
        Vector2 move = movementAction.action.ReadValue<Vector2>();

        handPoint = transform.position + grabOffset;

        if (move.y > 0) _rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);
        if (move.y < 0) _rb.AddForce(-orientation.forward * forwardThrustForce * Time.deltaTime);

        lineRenderer.SetPosition(1, handPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + detectOffset, hookRange);
        Gizmos.DrawSphere(transform.position + grabOffset, 0.1f);
    }
}
