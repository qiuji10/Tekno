using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigame_Speaker : MonoBehaviour
{
    [SerializeField] private InputActionReference circleAction;
    [SerializeField] private InputActionReference crossAction;
    [SerializeField] private InputActionReference squareAction;
    [SerializeField] private InputActionReference triangleAction;

    public KeyInput key = KeyInput.None;
    public Vector2 touchPoint;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        circleAction.action.performed += OnPressed;
        crossAction.action.performed += OnPressed;
        squareAction.action.performed += OnPressed;
        triangleAction.action.performed += OnPressed;
    }

    private void OnDisable()
    {
        circleAction.action.performed -= OnPressed;
        crossAction.action.performed -= OnPressed;
        squareAction.action.performed -= OnPressed;
        triangleAction.action.performed -= OnPressed;
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;
        KeyInput inputKey = (KeyInput)Enum.Parse(typeof(KeyInput), actionName, ignoreCase: true);

        if (inputKey == key)
        {
            // TODO: Offset in future for calibration
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, touchPoint))
            {
                Debug.Log("<color=green>success</color>");
            }
            else
            {
                Debug.Log("<color=red>fail</color>");
            }
        }
        else
        {
            Debug.Log("<color=red>wrong key</color>");
        }
        
    }
}
