using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : EnemyBaseState
{
    private EnemyStateManager stateManager;

    [Header("Player Detection")]
    public List<GameObject> detectedObjects;
    public SphereCollider detectionZone;

    void Start()
    {
        stateManager = GetComponent<EnemyStateManager>();
        detectionZone = GetComponentInChildren<SphereCollider>();
    }

    void Update()
    {
        detectedObjects.Clear();

        Collider[] overlappedColliders = Physics.OverlapSphere(detectionZone.transform.position, detectionZone.radius);
        foreach (Collider overlappedCollider in overlappedColliders)
        {
            if (overlappedCollider.CompareTag("Player"))
            {
                detectedObjects.Add(overlappedCollider.gameObject);
            }
        }

    }

    public override void Construct()
    {
        base.Construct();
    }

    public override Vector3 ImplementMovement()
    {
        return base.ImplementMovement();

        
    }

    public override void Transition()
    {
        base.Transition();
        // to transition use stateManager.ChangeState(GetComponent<StateToSwitch>());
    }
}
