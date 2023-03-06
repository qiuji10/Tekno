using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float verticalSpeed = 0.5f;     // Speed of floating
    public float floatHeight = 0.5f;    // Maximum height of floating
    public GameObject player;           // Reference to the player object

    private Vector3 basePosition;       // Base position for floating
    private float timeOffset;           // Random time offset for floating

    void Start()
    {
        basePosition = player.transform.position;

        // Add a random time offset to avoid all objects floating at the same time
        timeOffset = Random.Range(0.0f, 2.0f * Mathf.PI);
    }

    void Update()
    {
        // Calculate the new Y position based on the player's Y position and the floating effect
        float newY = basePosition.y + floatHeight * Mathf.Sin((Time.time + timeOffset) * verticalSpeed);

        // Set the new position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
