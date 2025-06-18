using UnityEngine;

public class CopperPickaxe : Pickaxe
{
    new public CopperPickaxe_ItemData data { get; private set; }
    public CopperPickaxe Set(CopperPickaxe_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}