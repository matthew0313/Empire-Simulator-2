using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TopLayer<T, ValT> : Layer<T, ValT> where ValT : FSMVals
{
    public Action onFSMChange;
    protected override TopLayer<T, ValT> root => this;
    readonly ValT m_values;
    protected override ValT values => m_values;
    public TopLayer(T origin, ValT values) : base(origin, null)
    {
        m_values = values;
    }
    public void AlertStateChange() => onFSMChange?.Invoke();
    public override string GetFSMPath()
    {
        return $"Top->{currentState.GetFSMPath()}";
    }
}
