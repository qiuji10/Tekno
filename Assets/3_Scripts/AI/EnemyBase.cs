using NaughtyAttributes;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _anim;
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    [Button]
    public void FreeEnemy()
    {
        GraphOwner owner = GetComponent<GraphOwner>();
        IBlackboard blackboard = owner.graph.blackboard;
        blackboard.SetVariableValue("FreeEnemy", true);
    }
}
