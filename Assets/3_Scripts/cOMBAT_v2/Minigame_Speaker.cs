using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI.Extensions;

public class Minigame_Speaker : MonoBehaviour
{
    [SerializeField] private InputActionReference circleAction;
    [SerializeField] private InputActionReference crossAction;
    [SerializeField] private InputActionReference squareAction;
    [SerializeField] private InputActionReference triangleAction;

    public KeyInput key = KeyInput.None;
    public Vector2 touchPoint, prevTouchPoint;
    public bool startTrace, failed;
    public float offsetTime = 0.1f, lastHitTime, timeToInput, beatToInput;
    public int totalInputNeeded, successInput;
    public int totalBeat, currentBeat;

    public BeatPoint beatPoint;

    [Header("Visuals")]
    [SerializeField] private UIParticleSystem successParticle;
    [SerializeField] private UIParticleSystem failParticle;

    private bool detectedEnterRegion;

    public event Action OnHitFailure, OnComboSuccess;

    public LTDescr speakerMovement { get; set; }
    private RectTransform rect;
    private MinigameData data;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        data = MinigameData.Instance;
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
        if (startTrace && !failed)
        {
            // TODO: Offset in future for calibration
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, touchPoint) && key != KeyInput.None)
            {
                detectedEnterRegion = true;
            }
            else
            {
                if (detectedEnterRegion && !RectTransformUtility.RectangleContainsScreenPoint(rect, prevTouchPoint))
                {
                    //Debug.Log($"{lastHitTime} / {TempoManager._lastBeatTime}, {offsetTime} <color=red>out of time</color>");
                    OnHitFailure?.Invoke();
                    data.promptText.text = "<color=green>out of time</color>";
                    failParticle.StartParticleEmission();
                    detectedEnterRegion = false;
                    startTrace = false;
                    failed = true;
                    LeanTween.cancel(gameObject);
                }
            }

            if (currentBeat == totalBeat)
            {
                if (Time.time > TempoManager._lastBeatTime + (60f / 140f) + timeToInput + offsetTime)
                {
                    //Debug.Log($"{lastHitTime} / {TempoManager._lastBeatTime}, {offsetTime} <color=red>out of time</color>");
                    OnHitFailure?.Invoke();
                    data.promptText.text = "<color=red>out of time</color>";
                    failParticle.StartParticleEmission();
                    detectedEnterRegion = false;
                    startTrace = false;
                    failed = true;
                }
            }
        }
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (!startTrace) return;

        float expectedBeatTime = Mathf.Floor((Time.time / (60f / 140f)) + 0.5f) * (60f / 140f);
        float currentInputTime = Time.time;
        float currentDelay = currentInputTime - expectedBeatTime;
        float inputDelay = currentDelay * 1000f; // Convert to milliseconds
        Debug.Log("Input Delay: " + inputDelay.ToString("F2") + "ms");

        string actionName = context.action.name;
        KeyInput inputKey = (KeyInput)Enum.Parse(typeof(KeyInput), actionName, ignoreCase: true);

        if (Time.time > TempoManager._lastBeatTime + (60f / 140f) - offsetTime - offsetTime && Time.time < TempoManager._lastBeatTime + (60f / 140f) + timeToInput + offsetTime)
        //if (Time.time > TempoManager._lastBeatTime - offsetTime && Time.time < TempoManager._lastBeatTime + timeToInput + offsetTime)
        {
            if (inputKey == key)
            {
                // TODO: Offset in future for calibration
                //if (RectTransformUtility.RectangleContainsScreenPoint(rect, touchPoint))
                if (speakerMovement.ratioPassed >= 0.1f)
                {
                    //Debug.Log("<color=green>success</color>");
                    lastHitTime = Time.time;
                    startTrace = false;
                    detectedEnterRegion = false;
                    successParticle.StartParticleEmission();
                    successInput++;
                    data.promptText.text = "<color=blue>Hacking in progress...</color>";

                    if (successInput == totalInputNeeded)
                    {
                        data.promptText.text = "<color=blue>GREAT!</color>";
                        OnComboSuccess?.Invoke();
                    }
                }
                else
                {
                    //Debug.Log("<color=red>fail</color>");
                    data.promptText.text = "<color=red>Too early</color>";
                    OnHitFailure?.Invoke();
                    failParticle.StartParticleEmission();
                    startTrace = false;
                    detectedEnterRegion = false;
                    failed = true;
                    LeanTween.cancel(gameObject);
                }
            }
            else
            {
                //Debug.Log("<color=yellow>wrong key</color>");
                data.promptText.text = "<color=red>wrong input</color>";
                OnHitFailure?.Invoke();
                failParticle.StartParticleEmission();
                startTrace = false;
                detectedEnterRegion = false;
                failed = true;
                LeanTween.cancel(gameObject);
            }
        }
        else
        {
            //Debug.Log("<color=cyan>out of time range</color>");
            data.promptText.text = "<color=yellow>out of time</color>";
            OnHitFailure?.Invoke();
            failParticle.StartParticleEmission();
            startTrace = false;
            detectedEnterRegion = false;
            failed = true;
            LeanTween.cancel(gameObject);
        }
    }
}
