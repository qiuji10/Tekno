using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform destination; // the destination node
    public float moveSpeed = 5.0f; // the speed with which to move the player towards the destination

    [SerializeField] private bool isTeleporting = false; // flag to indicate if the player is currently being teleported
    [SerializeField] private bool insideTrigger = false; // flag to indicate if the player is currently being teleported

    // NEW: Define the button that triggers teleportation

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player is in");
            // NEW: Check if the teleport button is pressed
            insideTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player is in");
            // NEW: Check if the teleport button is pressed
            insideTrigger = false;
        }
    }

    private void Start()
    {
        if (destination == null)
        {
            destination = transform;
        }
    }

    private void FixedUpdate()
    {
        if (insideTrigger)
        {
            if (Input.GetKey(KeyCode.Alpha5))
            {
                isTeleporting= true;
            }
        }

        // If the player is being teleported, move them towards the destination
        if (isTeleporting)
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Rigidbody playerRigidbody = player.transform.parent.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;

            // Calculate the direction to move the player in
            Vector3 moveDirection = (destination.position - playerRigidbody.position).normalized;

            // Move the player in the calculated direction at the specified speed
            playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * moveSpeed * Time.deltaTime);

            // If the player has reached the destination, stop teleporting them
            float distanceToDestination = Vector3.Distance(playerRigidbody.position, destination.position);
            if (distanceToDestination < 1.5f)
            {
                isTeleporting = false;
                playerRigidbody.isKinematic = false;
            }
        }

    }
}
