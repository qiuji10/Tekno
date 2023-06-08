using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnyKeyEvent : MonoBehaviour
{
    [SerializeField] private InputActionReference anyKey;

    [SerializeField] private UnityEvent OnAnyKeyActivated;

    private bool pressed;

    private void OnEnable()
    {
        anyKey.action.performed += Action_performed;
    }

    private void OnDisable()
    {
        anyKey.action.performed -= Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (pressed) return;

        pressed = true;
        OnAnyKeyActivated?.Invoke();
    }
}
