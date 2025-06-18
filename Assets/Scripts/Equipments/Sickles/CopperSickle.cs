using UnityEngine;

public class CopperSickle : Sickle
{
    new public CopperSickle_ItemData data { get; private set; }
    public CopperSickle Set(CopperSickle_ItemData data)
    {
        base.Set(data);
        this.data = data;
        return this;
    }
}