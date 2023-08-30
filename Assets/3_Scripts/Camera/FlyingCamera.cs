using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{

    [SerializeField] private Camera flyCam;
    [SerializeField] private CinemachineFreeLook playerCam;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private bool isFlyCam = false;
    [SerializeField] private bool isPlayerCam = true;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private Transform cameraTransform;

    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    private void Start()
    {
        cameraTransform = flyCam.transform;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isPlayerCam)
        {
            flyCam.gameObject.SetActive(true);
            isFlyCam = true;
            flyCam.enabled = true;
            isPlayerCam = false;
            playerCam.enabled = false;
            playerController.enabled = false;

        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraTransform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        cameraTransform.GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        // Get the input axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on camera's forward and right vectors
        Vector3 movementDirection = (cameraTransform.forward * verticalInput + cameraTransform.right * horizontalInput).normalized;

        // Apply the horizontal movement
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        // Check for vertical input (upward movement)
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }
        // Check for vertical input (downward movement)
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.Translate(-Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }
    }

}
