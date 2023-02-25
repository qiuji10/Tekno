using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public float verticalSpeed = 0.5f;     // Speed of floating
    public float floatHeight = 0.5f;    // Maximum height of floating
    private Vector3 startPosition;      // Starting position of the object
    //[SerializeField] private GameObject orientation;

    void Start()
    {
        startPosition = transform.position;   // Store starting position
    }

    void Update()
    {
        float newPosition = startPosition.y + (floatHeight * Mathf.Sin(Time.time * verticalSpeed));  // Calculate new y position
        transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);   // Set new position

        //transform.rotation = Quaternion.Slerp(transform.rotation, orientation.transform.rotation, verticalSpeed * Time.deltaTime);
    }
}
