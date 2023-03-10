using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EventInvoker : MonoBehaviour
{
    [SerializeField] private InputActionReference interactKey;

    [SerializeField] private GameObject prompt;
    [SerializeField] private UnityEvent OnInteract;

    private bool inRange;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        interactKey.action.performed += Interact;
    }

    private void OnDisable()
    {
        interactKey.action.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (inRange)
        {
            OnInteract?.Invoke();
            prompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(true);
            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (inRange && prompt != null)
            {
                prompt.transform.rotation = Quaternion.LookRotation(prompt.transform.position - cam.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(false);
            inRange = false;
        }
    }
}
