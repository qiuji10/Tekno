using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public class FirstSelected : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private bool selected;

    private void Start()
    {
        InputSystem.onAnyButtonPress.Call(ctrl =>
        {
            if (ctrl.device is Gamepad gamepad && !selected)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected);
                selected = true;
            }
        });
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && Gamepad.current != null && Gamepad.current.leftStick.IsActuated())
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
            selected = true;
        }
        else if (EventSystem.current.currentSelectedGameObject == null && selected)
        {
            selected = false;
        }
    }
}
