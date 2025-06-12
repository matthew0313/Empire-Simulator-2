using UnityEngine;

public class TestPickaxe : Pickaxe
{
    new public TestPickaxe_ItemData data { get; private set; }
    public TestPickaxe Set(TestPickaxe_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}