using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnyKeyEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAnyKeyActivated;

    private bool pressed;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (pressed) return;

            pressed = true;
            OnAnyKeyActivated?.Invoke();
        }
    }
}
