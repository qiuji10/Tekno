using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookAbility : MonoBehaviour
{
    [Header("Hook Settings")]
    [SerializeField] private float hookRange = 10;
    [SerializeField] private float horizontalThrustForce;
    [SerializeField] private float forwardThrustForce;

    [SerializeField] private float anchor = -4;
    [SerializeField] private float maxDistance = 5;

    [SerializeField] private Vector3 hookPoint;
    [SerializeField] private Vector3 handPoint;
    [SerializeField] private Vector3 grabOffset;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private InputActionReference hookAction;
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference shrinkAction;

    private PlayerController _playerController;
    private Rigidbody _rb;
    private SpringJoint _joint;

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
    }

    private void OnDisable()
    {
        hookAction.action.performed -= Hook;
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
            isHooking = false;
            _playerController.MoveSpeed = oriMoveSpeed;
            lineRenderer.positionCount = 0;
            Destroy(_joint);
            _joint = null;
        }
        else
        {
            //_playerController.enabled = false;
            
            Collider[] collideData = Physics.OverlapSphere(transform.position, hookRange);

            foreach (Collider collide in collideData) 
            { 
                if (collide.CompareTag("Hook") && collide.TryGetComponent(out Rigidbody rb))
                {
                    _joint = gameObject.AddComponent<SpringJoint>();
                    _joint.anchor = new Vector3(0, anchor, 0);
                    _joint.autoConfigureConnectedAnchor = false;
                    _joint.connectedBody = rb;

                    float distanceFromPoint = Vector3.Distance(transform.position, rb.position);

                    //The distance grapple will try to keep from grapple point. 
                    _joint.maxDistance = maxDistance;
                    _joint.minDistance = distanceFromPoint * 0.25f;

                    //Adjust these values to fit your game.
                    _joint.spring = 4.5f;
                    _joint.damper = 7f;
                    _joint.massScale = 4.5f;
                    

                    oriMoveSpeed = _playerController.MoveSpeed;
                    _playerController.MoveSpeed *= 1.5f;

                    hookPoint = rb.position;

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

        if (move.x > 0) _rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if (move.x < 0) _rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        if (move.y > 0) _rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);
        if (move.y < 0)
        {
            Vector3 direction = (hookPoint - transform.position).normalized;

            _rb.AddForce(direction * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, hookPoint);

            //The distance grapple will try to keep from grapple point. 
            _joint.maxDistance = distanceFromPoint * 0.8f;
            _joint.minDistance = distanceFromPoint * 0.25f;
        }

        lineRenderer.SetPosition(1, handPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hookRange);
        Gizmos.DrawSphere(transform.position + grabOffset, 0.1f);
        
    }
}
