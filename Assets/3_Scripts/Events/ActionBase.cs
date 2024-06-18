using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ActionEventBase
{
}

internal class ActionEvent : ActionEventBase
{
    private event Action m_Action;

    public void Initialize(Action action)
    {
        this.m_Action = action;
    }

    public void Invoke()
    {
        this.m_Action();
    }

    public bool IsAction(Action action)
    {
        return this.m_Action == action;
    }
}

internal class ActionEvent<T1> : ActionEventBase
{
    private event Action<T1> m_Action;

    public void Initialize(Action<T1> action)
    {
        this.m_Action = action;
    }

    public void Invoke(T1 arg1)
    {
        this.m_Action(arg1);
    }

    public bool IsAction(Action<T1> action)
    {
        return this.m_Action == action;
    }
}