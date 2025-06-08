using UnityEngine;

public abstract class Axe : Equipment
{
    new public AxeData data { get; private set; }
    public void Set(AxeData data)
    {
        base.Set(data);
        this.data = data;
    }
}