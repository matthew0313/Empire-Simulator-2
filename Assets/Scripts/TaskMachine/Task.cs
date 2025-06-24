using UnityEngine;

public class Task<T>
{
    protected T origin { get; private set; }
    protected TaskMachine<T> machine { get; private set; }
    public virtual void OnTaskStart(T origin, TaskMachine<T> machine)
    {
        this.origin = origin;
        this.machine = machine;
    }
    public virtual void OnTaskUpdate() { }
    public virtual void OnTaskEnd() { }
}