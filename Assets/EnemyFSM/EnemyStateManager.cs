using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance { get { return instance; } }
    public static EnemyStateManager instance;
    public Vector3 vectorMove;

    private EnemyBaseState currentState;

    private void Start()
    {
        currentState = GetComponent<EnemyIdle>();
        currentState.Construct();
    }

    public void ChangeState(EnemyBaseState newState)
    {
        currentState.Destruct();
        currentState = newState;
        currentState.Construct();
    }

    private void UpdateMovement()
    {
        vectorMove = currentState.ImplementMovement();
        currentState.Transition();
    }
}
