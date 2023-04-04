using Cinemachine;
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private GameObject item;
    [SerializeField] private CinemachineSmoothPath path;
    [SerializeField] private float density = 0.5f;

    [Header("New Path Settings")]
    [SerializeField] private float offsetDistance = -6;
    [SerializeField] private Transform cart;
    [SerializeField] private Transform offset;

    [Button]
    public void SpawnObjects()
    {
        Transform railwaysParent = null;

        if (transform.Find("Railways"))
        {
            DestroyImmediate(transform.Find("Railways").gameObject);
        }

        GameObject trailParent = new GameObject("Railways");
        trailParent.transform.parent = transform;
        railwaysParent = trailParent.transform;

        int numObjects = Mathf.CeilToInt(path.PathLength / density);
        float unitInterval = 1f / numObjects;
        float unit = 0f;

        for (int i = 0; i < numObjects; i++)
        {
            Vector3 position = path.EvaluatePositionAtUnit(unit, CinemachinePathBase.PositionUnits.Normalized);
            Quaternion rotation = path.EvaluateOrientationAtUnit(unit, CinemachinePathBase.PositionUnits.Normalized);

            GameObject pathObject = Instantiate(item, position, rotation, railwaysParent.transform);
            pathObject.isStatic = true;
            unit += unitInterval;
        }
    }


    [Button]
    public void CreateNewPathFromCurrentPath()
    {
        // Create a new game object to hold the new path
        //GameObject newPathObject = new GameObject("New Path");
        GameObject newPathObject = Instantiate(gameObject, Vector3.zero, Quaternion.identity);

        // Add a CinemachineSmoothPath component to the new game object
        //CinemachineSmoothPath newPath = newPathObject.AddComponent<CinemachineSmoothPath>();
        CinemachineSmoothPath newPath = newPathObject.GetComponent<CinemachineSmoothPath>();

        newPath.m_Looped = false;
        // Copy control points from original path
        newPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[path.m_Waypoints.Length];

        // Shift the new path's control points by the offset distance
        for (int i = 0; i < newPath.m_Waypoints.Length; i++)
        {
            Vector3 p = path.EvaluatePositionAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);
            Vector3 t = path.EvaluateTangentAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

            float d = Vector3.Distance(transform.position, cart.GetChild(0).position);

            //d = offset.position.x < 0 ? -d : d;

            Vector3 n = Vector3.Cross(t, Vector3.up).normalized * offsetDistance;

            //Add the new waypoint based on the calculated offset position
            newPath.m_Waypoints[i] = new CinemachineSmoothPath.Waypoint();
            newPath.m_Waypoints[i].position = (p + n);
            newPath.m_Waypoints[i].roll = path.m_Waypoints[i].roll;

        }

        // Generate the objects along the new path
        //PathGenerator newPathGenerator = newPathObject.AddComponent<PathGenerator>();
        PathGenerator newPathGenerator = newPathObject.GetComponent<PathGenerator>();
        newPathGenerator.path = newPath;
        newPathGenerator.density = density;
        newPathGenerator.item = item;
        newPathGenerator.SpawnObjects();
    }

    private void OnValidate()
    {
        offset.localPosition = new Vector3(offsetDistance, 0, 0);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < path.m_Waypoints.Length - 1; i++)
        {
            Vector3 p = path.EvaluatePositionAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);
            Vector3 t = path.EvaluateTangentAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);
            Vector3 n = Vector3.Cross(t, Vector3.up).normalized * offsetDistance;

            // Draw the tangent vector in red
            Gizmos.color = Color.red;
            Gizmos.DrawLine(p, p + t);

            // Draw the normal vector in blue
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(p, p + n);
        }
    }
}
