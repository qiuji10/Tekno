using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvoker : MonoBehaviour
{
    [SerializeField] UnityEvent OnAwake;
    [SerializeField] UnityEvent OnStart;
    [SerializeField] UnityEvent OnEnableEvent;
    [SerializeField] UnityEvent OnDisableEvent;
    [SerializeField] UnityEvent OnDestroyEvent;

    void Awake()
    {
        OnAwake?.Invoke();
    }

    void Start()
    {
        OnStart?.Invoke();
    }

    void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }

    void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
