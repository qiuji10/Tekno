using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private float offsetBeatTime = 0.3f;

    private void OnEnable()
    {
        teleportAction.action.performed += Teleport;
    }

    private void OnDisable()
    {
        teleportAction.action.performed -= Teleport;
    }

    private void Teleport(InputAction.CallbackContext context)
    {
        Collider[] collideData = Physics.OverlapSphere(transform.position, teleportRange);
        Transform nextTeleportPoint = null, prevTeleportPoint = null;

        foreach (Collider collide in collideData)
        {
            if (collide.TryGetComponent(out TeleportNode tpNode))
            {
                if (tpNode.nextTeleportPoint != null)
                {
                    nextTeleportPoint = tpNode.nextTeleportPoint;
                }

                if (tpNode.prevTeleportPoint != null)
                {
                    prevTeleportPoint = tpNode.prevTeleportPoint;
                }
                break;
            }
        }

        if (Time.time > TempoManager._lastBeatTime - offsetBeatTime && Time.time < TempoManager._lastBeatTime + offsetBeatTime)
        {
            if (nextTeleportPoint != null)
            {
                transform.position = nextTeleportPoint.position;
            }
        }
        else
        {
            if (prevTeleportPoint != null)
            {
                transform.position = prevTeleportPoint.position;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
