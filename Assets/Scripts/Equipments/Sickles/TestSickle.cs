using UnityEngine;

public class TestSickle : Sickle
{
    new public TestSickle_ItemData data { get; private set; }
    public TestSickle Set(TestSickle_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}