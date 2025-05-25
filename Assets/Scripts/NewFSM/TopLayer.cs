using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TopLayer<T> : Layer<T>
{
    public Action onFSMChange;
    protected override TopLayer<T> root => this;
    readonly FSMVals m_values;
    protected override FSMVals values => m_values;
    public TopLayer(T origin, FSMVals values) : base(origin, null)
    {
        m_values = values;
    }
    public void AlertStateChange() => onFSMChange?.Invoke();
    public override string GetFSMPath()
    {
        return $"Top->{currentState.GetFSMPath()}";
    }
}
