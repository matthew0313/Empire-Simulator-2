using UnityEngine;

public abstract class Sickle : Equipment
{
    new public SickleData data { get; private set; }
    public void Set(SickleData data)
    {
        base.Set(data);
        this.data = data;
    }
}