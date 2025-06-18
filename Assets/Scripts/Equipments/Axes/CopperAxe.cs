using UnityEngine;

public class CopperAxe : Axe
{
    new public CopperAxe_ItemData data { get; private set; }
    public CopperAxe Set(CopperAxe_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}