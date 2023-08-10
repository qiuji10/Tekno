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

    //private void Update()
    //{
    //    if (transformA == null || transformB == null) return;

    //    // Calculate the distance between the two transforms
    //    actualDistance = Vector3.Distance(transformA.position, transformB.position);

    //    // Round the positions to the nearest integer
    //    Vector3 roundedPositionA = new Vector3(Mathf.RoundToInt(transformA.position.x), Mathf.RoundToInt(transformA.position.y), Mathf.RoundToInt(transformA.position.z));
    //    Vector3 roundedPositionB = new Vector3(Mathf.RoundToInt(transformB.position.x), Mathf.RoundToInt(transformB.position.y), Mathf.RoundToInt(transformB.position.z));

    //    // Restrict the movement of transformA if it exceeds the distance
    //    if (actualDistance > distance)
    //    {
    //        Vector3 direction = (roundedPositionB - roundedPositionA).normalized;

    //        // Check if transformA is locked
    //        if (lockTransformA)
    //        {
    //            // Move transformB and keep transformA locked
    //            transformB.position = roundedPositionA + direction * distance;
    //        }
    //        else
    //        {
    //            // Move transformA
    //            transformA.position = roundedPositionB - direction * distance;
    //        }
    //    }

    //    // Calculate the distance again after the potential changes
    //    actualDistance = Vector3.Distance(transformA.position, transformB.position);

    //    // Restrict the movement of transformB if it exceeds the distance
    //    if (actualDistance > distance)
    //    {
    //        Vector3 direction = (roundedPositionA - roundedPositionB).normalized;

    //        // Check if transformB is locked
    //        if (lockTransformB)
    //        {
    //            // Move transformA and keep transformB locked
    //            transformA.position = roundedPositionB + direction * distance;
    //        }
    //        else
    //        {
    //            // Move transformB
    //            transformB.position = roundedPositionA - direction * distance;
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        if (transformA == null || transformB == null) return;

        actualDistance = Vector3.Distance(transformA.position, transformB.position);

        // Visualize the distance between the two transforms with a Gizmos line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transformA.position, transformB.position);
    }
}
