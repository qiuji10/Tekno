using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.PlayerInput;

public class EventManager : MonoBehaviour
{
    public const string GAMEPLAY_INPUT = "OnEnableGameplayInput";

    public static Dictionary<string, List<ActionEventBase>> actions = new();

    public static void RegisterEvent(string eventName, Action action)
    {
        ActionEvent actionEvent = new ActionEvent();
        actionEvent.Initialize(action);

        if (actions.TryGetValue(eventName, out var value))
        {
            value.Add(actionEvent);
            return;
        }

        value = new List<ActionEventBase>() { actionEvent };
        actions.Add(eventName, value);
    }

    public static void RegisterEvent<T1>(string eventName, Action<T1> action)
    {
        ActionEvent<T1> actionEvent = new ActionEvent<T1>();
        actionEvent.Initialize(action);

        if (actions.TryGetValue(eventName, out var value))
        {
            value.Add(actionEvent);
            return;
        }

        value = new List<ActionEventBase>() { actionEvent };
        actions.Add(eventName, value);
    }

    public static void UnregisterEvent(string eventName, Action action)
    {
        if (actions.TryGetValue(eventName, out var value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                var actionEvent = value[i] as ActionEvent;

                if (actionEvent.IsAction(action))
                    value.Remove(actionEvent);
            }
        }
    }

    public static void UnregisterEvent<T1>(string eventName, Action<T1> action)
    {
        if (actions.TryGetValue(eventName, out var value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                var actionEvent = value[i] as ActionEvent<T1>;

                if (actionEvent.IsAction(action))
                    value.Remove(actionEvent);
            }
        }
    }

    public static void ExecuteEvent(string eventName)
    {
        if (actions.TryGetValue(eventName, out var value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                ((ActionEvent)value[i]).Invoke();
            }
        }
    }

    public static void ExecuteEvent<T1>(string eventName, T1 arg1)
    {
        if (actions.TryGetValue(eventName, out var value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                ((ActionEvent<T1>)value[i]).Invoke(arg1);
            }
        }
    }

    private void OnDisable()
    {
        if (!(base.gameObject != null) || base.gameObject.activeSelf)
        {
            ClearTable();
        }
    }

    private void OnDestroy()
    {
        ClearTable();
    }

    private void ClearTable()
    {
        actions.Clear();
    }
}
