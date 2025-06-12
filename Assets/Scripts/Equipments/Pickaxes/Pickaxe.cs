using UnityEngine;

public abstract class Pickaxe : Equipment
{
    new public PickaxeData data { get; private set; }
    public void Set(PickaxeData data)
    {
        base.Set(data);
        this.data = data;
    }
}