using UnityEngine;

public abstract class FoodData : ItemData, ISavable
{
    [SerializeField] float energyRestored = 10.0f;
    public int selectionPriority = 0;
    public virtual void OnConsume(NPC consumer)
    {
        consumer.energy += energyRestored;
    }
    public void Save(SaveData data)
    {
        data.itemSettings[this] = new();
        data.itemSettings[this].integers["selectionPriority"] = selectionPriority;
    }
    public void Load(SaveData data)
    {
        if(data.itemSettings.TryGetValue(this, out DataUnit tmp))
        {
            selectionPriority = tmp.integers["selectionPriority"];
        }
    }
}