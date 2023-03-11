using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private float offsetBeatTime = 0.3f;

    [SerializeField] private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

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
                    Debug.LogWarning("teleport times");
                    nextTeleportPoint = tpNode.nextTeleportPoint;
                }

                if (tpNode.prevTeleportPoint != null)
                {
                    prevTeleportPoint = tpNode.prevTeleportPoint;
                }
                break;
            }
        }

        _rb.isKinematic = true;

        if (Time.time > TempoManager._lastBeatTime - offsetBeatTime && Time.time < TempoManager._lastBeatTime + offsetBeatTime)
        {
            if (nextTeleportPoint != null)
            {
                LeanTween.moveLocal(gameObject, nextTeleportPoint.position, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => { _rb.isKinematic = false; });
            }
        }
        else
        {
            if (prevTeleportPoint != null)
            {
                LeanTween.moveLocal(gameObject, prevTeleportPoint.position, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => { _rb.isKinematic = false; });
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
