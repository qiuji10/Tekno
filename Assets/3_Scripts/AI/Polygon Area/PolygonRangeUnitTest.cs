using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System.Linq;

public class PolygonRangeUnitTest : MonoBehaviour
{
    [SerializeField] List<Transform> points = new List<Transform>();
    private List<Vector3> v3Points = new List<Vector3>();

    private void Start()
    {
        foreach (var point in points)
        {
            v3Points.Add(point.position);
        }

        v3Points = PolygonUtility.AdjustPolygonSequence(v3Points);
    }

    private void OnDrawGizmosSelected()
    {
        if (points == null) return;

        //Vector3[] adjustedVertices = PolygonUtility.AdjustPolygonSequence(points.Select(t => t.position).ToArray());

        bool inPolygon = GeometryUtils.PointInPolygon(transform.position, points);

        if (inPolygon)
        {
            PolygonUtility.DebugIsInPolygon(points, Color.green);
        }
        else
        {
            PolygonUtility.DebugIsInPolygon(points, Color.red);
        }
    }
}
