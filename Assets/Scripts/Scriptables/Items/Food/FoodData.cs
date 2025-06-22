using UnityEngine;

public abstract class FoodData : ItemData, ISavable
{
    [SerializeField] float energyRestored = 10.0f;
    public int selectionPriority = 0;

    public override LangText itemDesc => base.itemDesc + new LangText()
    {
        en = $"\n\nEnergy Restored: {energyRestored}",
        kr = $"\n\n에너지 충전량: {energyRestored}"
    };
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