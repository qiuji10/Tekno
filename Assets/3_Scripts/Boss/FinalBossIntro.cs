using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class FinalBossIntro : MonoBehaviour
{
    [SerializeField] InputActionReference left;
    [SerializeField] InputActionReference up;
    [SerializeField] InputActionReference triangle;
    [SerializeField] InputActionReference circle;

    [SerializeField] UnityEvent OnGameStart;

    private Gamepad gamepad;

    private void OnEnable()
    {
        // Enable the input actions
        left.action.Enable();
        up.action.Enable();
        triangle.action.Enable();
        circle.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions
        left.action.Disable();
        up.action.Disable();
        triangle.action.Disable();
        circle.action.Disable();
    }

    private void Update()
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            if (AnyButtonPressedExcept(left, up, triangle, circle))
            {
                OnGameStart.Invoke();
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (Input.anyKeyDown)
            {
                OnGameStart.Invoke();
                gameObject.SetActive(false);
            }
        }

    }

    private bool AnyButtonPressedExcept(params InputActionReference[] exceptions)
    {
        // Check if any of the excluded buttons are pressed
        foreach (var exception in exceptions)
        {
            if (exception.action.triggered)
            {
                return false;
            }
        }

        // Check if any other gamepad button is pressed
        foreach (var control in gamepad.allControls)
        {
            if (control is ButtonControl button && button.wasPressedThisFrame)
            {
                bool isException = false;

                foreach (var exception in exceptions)
                {
                    if (exception.action.controls.Contains(control))
                    {
                        isException = true;
                        break;
                    }
                }

                if (!isException)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
