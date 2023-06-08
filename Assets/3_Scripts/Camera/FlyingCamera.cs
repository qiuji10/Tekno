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
    [SerializeField] private List<GameObject> UI = new List<GameObject>();

    [SerializeField] private float speed = 5f;
    [SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private Transform cameraTransform;

    public float movementSpeed = 10f;
    public float rotationSpeed = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        cameraTransform = flyCam.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) && isPlayerCam)
        {
            flyCam.gameObject.SetActive(true);
            isFlyCam = true;
            flyCam.enabled = true;
            isPlayerCam = false;
            playerCam.enabled = false;
            playerController.enabled = false;
            DisableUI(false);

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isFlyCam)
        {
            flyCam.gameObject.SetActive(false);
            isFlyCam = false;
            flyCam.enabled = false;
            isPlayerCam = true;
            playerCam.enabled = true;
            playerController.enabled = true;
            DisableUI(true);
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement.Normalize();
        transform.position += movement * movementSpeed * Time.deltaTime;

        // Rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX += mouseY * rotationSpeed;
        rotationY += mouseX * rotationSpeed;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(-rotationX, rotationY, 0f);
    }

    void DisableUI(bool option)
    {
        foreach (GameObject ui in UI)
        {
            ui.SetActive(option);
        }
    }

}
