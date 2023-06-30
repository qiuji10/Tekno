using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class DistanceRestriction : MonoBehaviour
{
    public Transform transformA;
    public Transform transformB;
    public float distance = 10;

    [ReadOnly]
    public float actualDistance;

    public bool lockTransformA;
    public bool lockTransformB;

    private void Update()
    {
        if (transformA == null || transformB == null) return;

        // Calculate the distance between the two transforms
        actualDistance = Vector3.Distance(transformA.position, transformB.position);

        // Restrict the movement of transformA if it exceeds the distance
        if (actualDistance > distance)
        {
            Vector3 direction = (transformB.position - transformA.position).normalized;

            // Check if transformA is locked
            if (lockTransformA)
            {
                // Move transformB and keep transformA locked
                transformB.position = transformA.position + direction * distance;
            }
            else
            {
                // Move transformA
                transformA.position = transformB.position - direction * distance;
            }
        }

        // Restrict the movement of transformB if it exceeds the distance
        if (actualDistance > distance)
        {
            Vector3 direction = (transformA.position - transformB.position).normalized;

            // Check if transformB is locked
            if (lockTransformB)
            {
                // Move transformA and keep transformB locked
                transformA.position = transformB.position + direction * distance;
            }
            else
            {
                // Move transformB
                transformB.position = transformA.position - direction * distance;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (transformA == null || transformB == null) return;

        // Visualize the distance between the two transforms with a Gizmos line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transformA.position, transformB.position);
    }
}
