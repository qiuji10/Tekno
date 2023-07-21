using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum EventInvokeType { Enter, Stay }

public class EventInvoker : MonoBehaviour
{
    [SerializeField, ShowIf("IS_STAY")] private InputActionReference interactKey;
    [SerializeField] private EventInvokeType eventType = EventInvokeType.Stay;
    [SerializeField] private Genre genre = Genre.All;

    [SerializeField, ShowIf("IS_STAY")] private GameObject prompt;
    [SerializeField] private UnityEvent OnInteract;
    [SerializeField] private UnityEvent OnInteractDelay;
    [SerializeField] private UnityEvent OnExit;

    [SerializeField] private bool triggerOnce;
    [SerializeField] private float triggerDelay;

    private bool inRange, triggerDisable;
    private Camera cam;

    private void Start()
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
        if (StanceManager.curTrack.genre != genre && genre != Genre.All) return;

        if (!triggerDisable && eventType == EventInvokeType.Stay && inRange)
        {
            if (triggerOnce)
            {
                triggerDisable = true;
                prompt.SetActive(false);
            }

            OnInteract?.Invoke();
            StartCoroutine(DelayEvent());
            //prompt.SetActive(false);
        }
    }

    public void Invoke_OnInteract()
    {
        OnInteract?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (StanceManager.curTrack.genre != genre && genre != Genre.All) return;

        if (!triggerDisable && other.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(true);

            if (eventType == EventInvokeType.Enter)
            {
                if (triggerOnce)
                {
                    triggerDisable = true;
                    if (prompt != null) prompt.SetActive(false);
                }

                OnInteract?.Invoke();
                StartCoroutine(DelayEvent());
            }

            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (StanceManager.curTrack.genre != genre && genre != Genre.All)
        {
            if (prompt != null) prompt.SetActive(false);
            OnExit?.Invoke();
            inRange = false;
            return;
        }

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
            OnExit?.Invoke();
            inRange = false;
        }
    }

    private IEnumerator DelayEvent()
    {
        yield return new WaitForSeconds(triggerDelay);
        OnInteractDelay?.Invoke();
    }

    private bool IS_ENTER() { return eventType == EventInvokeType.Enter; }
    private bool IS_STAY() { return eventType == EventInvokeType.Stay; }
}
