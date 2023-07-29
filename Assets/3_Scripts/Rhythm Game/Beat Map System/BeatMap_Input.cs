using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeatMap_Input : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeWindow;

    [Header("Action Reference")]
    [SerializeField] private InputActionReference leftAction;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference triAction;
    [SerializeField] private InputActionReference cirAction;

    [Header("Lane Reference")]
    [SerializeField] private GameObject[] lane;
    [SerializeField] private ParticleAttractorSpherical[] particles;
    [SerializeField] private RhythmGameMiniAmp[] amps;

    private Coroutine[] task = new Coroutine[4];

    public static Dictionary<Lane, NoteObject> inputData = new Dictionary<Lane, NoteObject>(4);
    public static event Action<Lane> OnNoteSuccess, OnLongNoteEnd;

    private void Awake()
    {
        inputData.Clear();

        inputData.Add(Lane.Lane1, null);
        inputData.Add(Lane.Lane2, null);
        inputData.Add(Lane.Lane3, null);
        inputData.Add(Lane.Lane4, null);

        leftAction.action.performed += OnKeyDown;
        upAction.action.performed += OnKeyDown;
        triAction.action.performed += OnKeyDown;
        cirAction.action.performed += OnKeyDown;

        leftAction.action.canceled += OnKeyUp;
        upAction.action.canceled += OnKeyUp;
        triAction.action.canceled += OnKeyUp;
        cirAction.action.canceled += OnKeyUp;

        OnNoteSuccess += SuccessEffect;
        OnLongNoteEnd += StopMiniAmpEffect;
    }

    private void OnDestroy()
    {
        inputData.Clear();

        leftAction.action.performed -= OnKeyDown;
        upAction.action.performed -= OnKeyDown;
        triAction.action.performed -= OnKeyDown;
        cirAction.action.performed -= OnKeyDown;

        leftAction.action.canceled -= OnKeyUp;
        upAction.action.canceled -= OnKeyUp;
        triAction.action.canceled -= OnKeyUp;
        cirAction.action.canceled -= OnKeyUp;

        OnNoteSuccess -= SuccessEffect;
        OnLongNoteEnd -= StopMiniAmpEffect;
    }

    public static void CallSuccess(Lane lane)
    {
        OnNoteSuccess?.Invoke(lane);
    }

    public static void CallLongNoteEnd(Lane lane)
    {
        OnLongNoteEnd?.Invoke(lane);
    }

    public void StopMiniAmpEffect(Lane lane)
    {
        amps[(int)lane].StopHitEffect();
    }

    public void SuccessEffect(Lane lane)
    {
        particles[(int)lane].Play();
    }

    private void OnKeyUp(InputAction.CallbackContext context)
    {
        int index = 0;

        switch (context.action.name)
        {
            case "Left":
                index = 0;
                break;
            case "Up":
                index = 1;
                break;
            case "Triangle":
                index = 2;
                break;
            case "Circle":
                index = 3;
                break;
        }

        if (task[index] != null)
            StopCoroutine(task[index]);

        lane[index].SetActive(false);

        NoteObject note = inputData[(Lane)index];

        if (note != null && note is NoteObject_Hold)
        {
            if ((note as NoteObject_Hold).percentage < 0.9)
            {
                (note as NoteObject_Hold).ToggleCollider(true);
            }
            else
            {
                (note as NoteObject_Hold).ToggleCollider(false);
            }
        }
    }

    private void OnKeyDown(InputAction.CallbackContext context)
    {
        int index = 0;

        switch (context.action.name)
        {
            case "Left":
                index = 0;
                break;
            case "Up":
                index = 1;
                break;
            case "Triangle":
                index = 2;
                break;
            case "Circle":
                index = 3;
                break;
        }

        if (task[index] != null)
            StopCoroutine(task[index]);

        task[index] = StartCoroutine(DetachInput(index));

        NoteObject note = inputData[(Lane)index];

        if (note != null && note is NoteObject_Hold)
        {
            (note as NoteObject_Hold).ToggleCollider(false);
        }
        else
        {
            StartCoroutine(EarlyWindow(index));
        }
    }

    private IEnumerator DetachInput(int index)
    {
        GameObject gameObject = lane[index];
        
        gameObject.SetActive(true);

        if (inputData[(Lane)index] != null)
        {
            if (inputData[(Lane)index] is NoteObject_Hold)
            {
                Debug.Log("yo");
                amps[index].StopHitEffect();
                yield return new WaitUntil(() => inputData[(Lane)index] == null);

                OnNoteSuccess?.Invoke((Lane)index);
                gameObject.SetActive(false);
                yield break;
            }
        }

        yield return new WaitForSeconds(timeWindow);
        gameObject.SetActive(false);

    }

    private IEnumerator EarlyWindow(int index)
    {
        float prevTime = Time.time;

        float windowTime = 0.096f;

        yield return new WaitUntil(() => Time.time > prevTime + windowTime || inputData[(Lane)index] != null);

        if (Time.time > prevTime + windowTime)
            yield break;

        NoteObject note = inputData[(Lane)index];

        if (note != null)
        {
            InputAction action = null;

            switch (index)
            {
                case 0:
                    action = leftAction.action;
                    break;
                case 1:
                    action = upAction.action;
                    break;
                case 2:
                    action = triAction.action;
                    break;
                case 3:
                    action = cirAction.action;
                    break;
            }

            if (action.IsPressed())
            {
                if (note != null && note is NoteObject_Hold)
                {
                    Debug.Log("yoyo");
                    StopCoroutine(task[index]);
                    lane[index].SetActive(true);
                    (note as NoteObject_Hold).ToggleCollider(false);
                    amps[index].StopHitEffect();
                    yield return new WaitUntil(() => inputData[(Lane)index] == null);
                    lane[index].SetActive(false);
                    OnNoteSuccess?.Invoke((Lane)index);
                }
            }
        }
    }
}
