using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpeaker : MonoBehaviour
{
    public float verticalSpeed = 0.5f;     // Speed of floating
    public float floatHeight = 0.5f;    // Maximum height of floating
    private Vector3 startPosition;      // Starting position of the object
    public InputReceiver receiveInput;
    public GameObject waypoint;
    public float moveSpeed = 5.0f;
    [SerializeField]private Transform bulletPos;

    void Start()
    {
        startPosition = transform.position;   // Store starting position
    }

    void Update()
    {
        float newPosition = startPosition.y + (floatHeight * Mathf.Sin(Time.time * verticalSpeed));  // Calculate new y position

        transform.position = new Vector3(waypoint.transform.position.x, newPosition, waypoint.transform.position.z);   // Set new position

        if(receiveInput.attack == true)
        {
            Debug.Log("shoot");
            Attack();
        }


    }

    public void Attack()
    {
        GameObject bullet = ObjectPooling.instance.GetPooledObject();

        if (bullet != null)
        {
            bullet.transform.position = bulletPos.position;
            bullet.SetActive(true);
            receiveInput.attack = false;
        }

    }
}
