using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _anim;

    [SerializeField] private GlobalBlackboard _blackboard;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    public void FreeEnemy()
    {
        
    }
}
