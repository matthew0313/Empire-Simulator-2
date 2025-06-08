using UnityEngine;

public class TestAxe : Axe
{
    new public TestAxe_ItemData data { get; private set; }
    public TestAxe Set(TestAxe_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}