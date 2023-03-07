using ParadoxNotion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Minigame_Speaker : MonoBehaviour
{
    [SerializeField] private InputActionReference circleAction;
    [SerializeField] private InputActionReference crossAction;
    [SerializeField] private InputActionReference squareAction;
    [SerializeField] private InputActionReference triangleAction;

    public KeyInput key = KeyInput.None;
    public Vector2 touchPoint;
    public bool startTrace, failed;
    public float offsetTime = 0.1f, lastHitTime, timeToInput;

    public LTDescr speakerMovement { get; set; }

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

    private void Update()
    {
        if (!failed && startTrace && lastHitTime < TempoManager._lastBeatTime - timeToInput - offsetTime)
        {
            failed = true;
            LeanTween.cancel(gameObject);
            Debug.Log($"{lastHitTime} / {TempoManager._lastBeatTime}, {offsetTime} <color=red>out of time</color>");
        }
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (failed)
        {
            Debug.Log("<color=red>out of time</color>");
            LeanTween.cancel(gameObject);
            return;
        }

        string actionName = context.action.name;
        KeyInput inputKey = (KeyInput)Enum.Parse(typeof(KeyInput), actionName, ignoreCase: true);

        if (Time.time > TempoManager._lastBeatTime - offsetTime && Time.time < TempoManager._lastBeatTime + timeToInput + offsetTime)
        {
            if (inputKey == key)
            {
                // TODO: Offset in future for calibration
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, touchPoint))
                {
                    Debug.Log("<color=green>success</color>");
                    lastHitTime = Time.time;
                    startTrace = false;
                }
                else
                {
                    Debug.Log("<color=red>fail</color>");
                    LeanTween.cancel(gameObject);
                }
            }
            else
            {
                Debug.Log("<color=yellow>wrong key</color>");
                LeanTween.cancel(gameObject);
            }
        }
        else
        {
            Debug.Log("<color=cyan>out of time range</color>");
            LeanTween.cancel(gameObject);
        }
    }
}
