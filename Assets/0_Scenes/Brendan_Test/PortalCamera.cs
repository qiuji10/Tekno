using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform playerCamera;
    public Transform portal;
    public Transform otherPortal;

    private void Update()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
        transform.position = portal.position + playerOffsetFromPortal /50;

        float angularDiffPortalsRotate = Quaternion.Angle(portal.rotation, otherPortal.rotation);

        Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDiffPortalsRotate, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * playerCamera.forward;
        transform.rotation =  Quaternion.LookRotation(newCameraDirection , Vector3.up);
    }
}
