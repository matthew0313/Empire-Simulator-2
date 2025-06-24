using UnityEngine;

public class TaskMachine<T>
{
    readonly T origin;
    public TaskMachine(T origin)
    {
        this.origin = origin;
    }
    Task<T> currentTask = null;
    public void StartTask(Task<T> task)
    {
        if (currentTask != null) currentTask.OnTaskEnd();
        currentTask = task;
        currentTask.OnTaskStart(origin, this);
    }
    public void Update()
    {
        if(currentTask != null) currentTask.OnTaskUpdate();
    }
    public void EndTask()
    {
        if (currentTask == null) return;
        currentTask.OnTaskEnd();
        currentTask = null;
    }
}