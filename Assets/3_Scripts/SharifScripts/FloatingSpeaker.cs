using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 10f;
    public float smoothTimeXZ = 0.3f;
    public float smoothTimeY = 0.6f;
    private Vector3 velocityXZ = Vector3.zero;
    private Vector3 velocityY = Vector3.zero;
    private Vector3 targetPosition;

    void Update()
    {
        // Set target position for movement
        targetPosition = player.transform.position;

        // Move the object towards the target position on the x and z axes
        Vector3 targetPositionXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPositionXZ, ref velocityXZ, smoothTimeXZ);

        // Move the object towards the target position on the y axis
        Vector3 targetPositionY = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPositionY, ref velocityY, smoothTimeY);
    }
}
