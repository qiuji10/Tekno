using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportAbility : MonoBehaviour
{
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private ChargingSystem charge;
    [SerializeField] public MotherNode motherNode;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private float offsetBeatTime = 0.3f;
    [SerializeField] private Transform electricVFX;
    private void OnEnable()
    {
        teleportAction.action.performed += Teleport;
        LeanTween.reset();
    }

    private void OnDisable()
    {
        teleportAction.action.performed -= Teleport;
    }

    private void Teleport(InputAction.CallbackContext context)
    {
        if (charge.isMaxCharge && motherNode != null)
        {
            motherNode.canTeleport();
        }
        //if (charge.isMaxCharge && Time.time > TempoManager._lastBeatTime - offsetBeatTime && Time.time < TempoManager._lastBeatTime + offsetBeatTime)
        //{
        //    Collider[] collideData = Physics.OverlapSphere(transform.position, teleportRange);
        //    Transform nextTeleportPoint = null, prevTeleportPoint = null;
        //    Vector3 teleportDirection = Vector3.zero;
        //    int teleportNodeCount = 0;

        //    foreach (Collider collide in collideData)
        //    {
        //        if (collide.TryGetComponent(out TeleportNode tpNode))
        //        {
        //            if (tpNode.nextTeleportPoint != null)
        //            {
        //                nextTeleportPoint = tpNode.nextTeleportPoint;
        //                teleportDirection += nextTeleportPoint.position - transform.position;
        //                teleportNodeCount++;
        //            }

        //            if (tpNode.prevTeleportPoint != null)
        //            {
        //                prevTeleportPoint = tpNode.prevTeleportPoint;
        //                teleportDirection += prevTeleportPoint.position - transform.position;
        //                teleportNodeCount++;
        //            }

        //            break;
        //        }
        //    }

        //    if (nextTeleportPoint == null && prevTeleportPoint == null) return;
        //    Vector3 targetPosition = (nextTeleportPoint != null ? nextTeleportPoint.position : prevTeleportPoint.position);
        //    Vector3 direction = (targetPosition - transform.position).normalized;
        //    Vector3 centerPoint = Vector3.Lerp(transform.position, targetPosition, 0.5f);
        //    Transform vfx = Instantiate(electricVFX, targetPosition, Quaternion.LookRotation(direction));

        //    if (nextTeleportPoint != null)
        //    {
        //        LeanTween.move(gameObject, nextTeleportPoint, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx.gameObject, 0.35f));
        //    }
        //    else
        //    {
        //        LeanTween.move(gameObject, prevTeleportPoint, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx.gameObject, 0.35f));
        //    }
        //}
    }

    public void TeleportToNextNode(Transform targetTeleportPoint)
    {
        Vector3 direction = (targetTeleportPoint.position - transform.position).normalized;
        Vector3 centerPoint = Vector3.Lerp(transform.position, targetTeleportPoint.position, 0.5f);
        Transform vfx = Instantiate(electricVFX, targetTeleportPoint.position, Quaternion.LookRotation(direction));
        LeanTween.move(gameObject, targetTeleportPoint.position, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx.gameObject, 0.35f));
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
