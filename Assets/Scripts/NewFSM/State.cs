using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T, ValT> where ValT : FSMVals
{
    protected readonly T origin;
    public readonly Layer<T, ValT> parentLayer;
    protected virtual TopLayer<T, ValT> root => parentLayer.root;
    protected virtual ValT values => root.values;
    public State(T origin, Layer<T, ValT> parent)
    {
        this.origin = origin;
        this.parentLayer = parent;
    }
    public virtual void OnStateEnter()
    {
        root.AlertStateChange();
    }
    public virtual void RefreshState()
    {

    }
    public virtual void OnStateUpdate()
    {
        
    }
    public virtual void OnStateFixedUpdate()
    {

    }
    public virtual void OnStateExit()
    {

    }
    public virtual string GetFSMPath()
    {
        return parentLayer.GetStateName(this);
    }
}
