using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportWVFX : MonoBehaviour
{
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private float teleportRange = 5f;
    [SerializeField] private float offsetBeatTime = 0.3f;
    [SerializeField] private GameObject electricVFX;
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
        Collider[] collideData = Physics.OverlapSphere(transform.position, teleportRange);
        Transform nextTeleportPoint = null, prevTeleportPoint = null;
        Vector3 teleportDirection = Vector3.zero;
        int teleportNodeCount = 0;

        foreach (Collider collide in collideData)
        {
            if (collide.TryGetComponent(out TeleportNode tpNode))
            {
                if (tpNode.nextTeleportPoint != null)
                {
                    nextTeleportPoint = tpNode.nextTeleportPoint;
                    teleportDirection += nextTeleportPoint.position - transform.position;
                    teleportNodeCount++;
                }

                if (tpNode.prevTeleportPoint != null)
                {
                    prevTeleportPoint = tpNode.prevTeleportPoint;
                    teleportDirection += prevTeleportPoint.position - transform.position;
                    teleportNodeCount++;
                }

                break;
            }
        }

        if (nextTeleportPoint == null && prevTeleportPoint == null) return;

        Vector3 centerPoint = Vector3.Lerp(transform.position, nextTeleportPoint != null ? nextTeleportPoint.position : prevTeleportPoint.position, 0.5f);
        GameObject vfx = Instantiate(electricVFX, centerPoint, Quaternion.identity);
        Vector3 vfxDirection = teleportDirection / 2;
        vfxDirection.Normalize();
        Quaternion vfxRotation = Quaternion.Euler(vfxDirection);
        vfx.transform.rotation = vfxRotation;

        if (nextTeleportPoint != null)
        {
            LeanTween.move(gameObject, nextTeleportPoint, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx,0.35f));
        }
        else
        {
            LeanTween.move(gameObject, prevTeleportPoint, TempoManager.GetTimeToBeatCount(1f)).setOnComplete(() => Destroy(vfx, 0.35f));
        }

        
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }

    void DisabeleVFX()
    {
        electricVFX.SetActive(false);
    }
}
