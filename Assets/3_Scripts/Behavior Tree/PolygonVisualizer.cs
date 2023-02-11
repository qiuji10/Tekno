using System;
using UnityEngine;

public class PolygonVisualizer : MonoBehaviour
{
    public Transform[] polygonVertices;
    public bool showGizmos = true;

    public float threshold = 0.1f;

    private void Update()
    {
        Vector3[] adjustedWaypoints = PolygonUtility.AdjustPolygonSequence(
            Array.ConvertAll(polygonVertices, x => x.position));

        if (PolygonUtility.IsPointInPolygon(transform.position, adjustedWaypoints))
        {
            Debug.Log("Inside polygon.");
        }
        else
        {
            //Debug.Log("Out of bounds.");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || polygonVertices == null) return;

        Vector3[] adjustedVertices = PolygonUtility.AdjustPolygonSequence(Array.ConvertAll(polygonVertices, x => x.position));
        PolygonUtility.DrawPolygon(adjustedVertices);
    }
}
