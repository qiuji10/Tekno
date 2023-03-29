using UnityEngine;
using System.Collections.Generic;

public class SensorDetection : MonoBehaviour
{
    public float detectionRange = 5f;
    public bool showGizmos = true;

    private List<Collider> detectedColliders = new List<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (!detectedColliders.Contains(other))
        {
            detectedColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        detectedColliders.Remove(other);
    }

    public T GetNearestObject<T>()
    {
        T nearestObject = default(T);
        float nearestDistance = float.MaxValue;

        foreach (Collider detectedCollider in detectedColliders)
        {
            if (detectedCollider.TryGetComponent<T>(out T detectedObject))
            {
                float distanceToCollider = Vector3.Distance(transform.position, detectedCollider.transform.position);
                if (distanceToCollider < nearestDistance)
                {
                    nearestDistance = distanceToCollider;
                    nearestObject = detectedObject;
                }
            }
        }

        return nearestObject;
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
