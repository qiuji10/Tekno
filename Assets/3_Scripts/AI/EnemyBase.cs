using NaughtyAttributes;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IKnockable
{
    public DamageTrigger damageTrigger;

    [Header("Knockback Settings")]
    [SerializeField] private float afterKnockedWaitTime;
    [SerializeField] private float maxKnockbackSpeed;
    [SerializeField] private float decelerationRate;
    [SerializeField] Vector3 directionMuliplier;
    private LayerMask ignoreLayer;

    [Header("Ground Detection Settings")]
    [SerializeField] private float detectDistance;
    [SerializeField] private Vector3 groundDetectOffset;

    [Header("Breaking Objects")]
    [SerializeField] private BreakingObject vrHeadset;

    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Animator _anim;
    private GraphOwner _owner;
    private Vector3 lastFacing;
    private float velocity;
    private bool isKnockng, isFree;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _owner = GetComponent<GraphOwner>();
        _anim = GetComponentInChildren<Animator>();

        ignoreLayer = ~(1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player"));

        isKnockng = false;
    }

    private void Update()
    {
        Vector3 currentFacing = transform.forward;
        float currentAngularVelocity = Vector3.Angle(currentFacing, lastFacing) / Time.deltaTime; //degrees per second
        lastFacing = currentFacing;

        if (_agent.velocity != Vector3.zero)
        {
            float currentWeight = _anim.GetLayerWeight(1);
            _anim.SetLayerWeight(1, Mathf.SmoothDamp(currentWeight, 1, ref velocity, 0.1f));
        }
        else if (currentAngularVelocity > 1f)
        {
            _anim.SetLayerWeight(1, 0.5f);
        }
        else
        {
            float currentWeight = _anim.GetLayerWeight(1);
            _anim.SetLayerWeight(1, Mathf.SmoothDamp(currentWeight, 0, ref velocity, 0.1f));
        }
    }

    private void FixedUpdate()
    {
        if (isKnockng && !_agent.enabled)
        {
            if (Physics.Raycast(transform.position + groundDetectOffset, -Vector3.up, detectDistance, ignoreLayer))
            {
                isKnockng = false;
                transform.eulerAngles = Vector3.zero;
                _rb.velocity = Vector3.zero;
                //_rb.isKinematic = true;
                _rb.constraints = RigidbodyConstraints.None;
                _agent.enabled = true;
                _owner.RestartBehaviour();
            }
        }
    }

    public void Attack()
    {
        damageTrigger.TriggerCollider(true);
        StartCoroutine(ResetDamageTrigger());
    }

    IEnumerator ResetDamageTrigger()
    {
        yield return new WaitForSeconds(1f);
        damageTrigger.TriggerCollider(false);
    }

    [Button]
    public void FreeEnemy()
    {

        //IBlackboard blackboard = _owner.graph.blackboard;
        //blackboard.SetVariableValue("FreeEnemy", true);
        isFree = true;
        vrHeadset.BreakObjects();
        _owner.StopBehaviour();
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        if (_agent.enabled) _agent.isStopped = true;
        _anim.SetLayerWeight(1, 0);
        _anim.SetLayerWeight(2, 0);
    }

    public void Knock(Vector3 direction, float power)
    {
        if (!_agent.enabled || isFree) return;

        // knockback power should be 100
        StartCoroutine(KnockedLogic(direction, power));
    }

    private IEnumerator KnockedLogic(Vector3 direction, float power)
    {
        _anim.SetLayerWeight(1, 0);
        _anim.SetLayerWeight(2, 0);

        _anim.SetTrigger("IsKnocked");
        _owner.PauseBehaviour();

        if (_agent.enabled) _agent.isStopped = true;
        _agent.enabled = false;
        _rb.isKinematic = false;
        yield return null;
        _rb.velocity = Vector3.zero;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _rb.AddForce(new Vector3(direction.x * directionMuliplier.x, directionMuliplier.y, direction.z * directionMuliplier.z) * power, ForceMode.VelocityChange);

        // Limit maximum speed
        if (_rb.velocity.magnitude > maxKnockbackSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * maxKnockbackSpeed;
        }

        yield return new WaitForSeconds(afterKnockedWaitTime);
        isKnockng = true;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 offset = transform.position + groundDetectOffset;
        Vector3 detectEnd = new Vector3(offset.x, offset.y - detectDistance, offset.z);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(offset, detectEnd);
    }
}
