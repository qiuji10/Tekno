using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public Transform player;
    public Transform waypoint;
    public float moveSpeed = 10f;
    public float smoothTimeXZ = 0f;
    public float smoothTimeY = 0.2f;
    public float beatOffsetY = 5f;
    public float lookAtSmoothTime = 0f; // added variable for look at smoothing

    private Vector3 velocityXZ = Vector3.zero;
    private Vector3 velocityY = Vector3.zero;
    private Vector3 targetPosition;
    private bool isOnBeat = false;

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        isOnBeat = true;
    }

    void Update()
    {
        targetPosition = waypoint.position;
        Vector3 targetPositionXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPositionXZ, ref velocityXZ, smoothTimeXZ);

        Vector3 targetPositionY = new Vector3(transform.position.x, targetPosition.y, transform.position.z);

        if (isOnBeat)
        {
            targetPositionY.y -= beatOffsetY;
            isOnBeat = false;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPositionY, ref velocityY, smoothTimeY);
        Quaternion targetRotation = Quaternion.LookRotation(player.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime / lookAtSmoothTime);
    }
}
