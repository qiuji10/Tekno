using NaughtyAttributes;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    public DamageTrigger damageTrigger;

    private NavMeshAgent _agent;
    private Animator _anim;
    private Vector3 lastFacing;
    private float velocity;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
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
        GraphOwner owner = GetComponent<GraphOwner>();
        IBlackboard blackboard = owner.graph.blackboard;
        blackboard.SetVariableValue("FreeEnemy", true);
    }
}
