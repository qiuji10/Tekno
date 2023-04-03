using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum EventInvokeType { Enter, Stay }

public class EventInvoker : MonoBehaviour
{
    [SerializeField] private InputActionReference interactKey;
    [SerializeField] private EventInvokeType eventType = EventInvokeType.Stay;

    [SerializeField] private GameObject prompt;
    [SerializeField] private UnityEvent OnInteract;

    [SerializeField] private bool triggerOnce;

    private bool inRange, triggerDisable;
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
        if (!triggerDisable && eventType == EventInvokeType.Stay && inRange)
        {
            if (triggerOnce)
            {
                triggerDisable = true;
                prompt.SetActive(false);
            }

            OnInteract?.Invoke();
            //prompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerDisable && other.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(true);

            if (eventType == EventInvokeType.Enter)
            {
                if (triggerOnce)
                {
                    triggerDisable = true;
                    prompt.SetActive(false);
                }

                OnInteract?.Invoke();
            }

            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!triggerDisable && inRange && prompt != null)
            {
                prompt.transform.rotation = Quaternion.LookRotation(prompt.transform.position - cam.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(false);
            inRange = false;
        }
    }
}
