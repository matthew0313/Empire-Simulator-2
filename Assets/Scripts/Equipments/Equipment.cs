using UnityEngine;

public abstract class Equipment : PooledPrefab<Equipment>
{
    public EquipmentData data { get; private set; }
    public float durabilityLeft { get; private set; }
    public void Set(EquipmentData data)
    {
        this.data = data;
        durabilityLeft = data.durability;
    }
    public void LoseDurability(float amount)
    {
        durabilityLeft -= amount;
        if(durabilityLeft <= 0)
        {
            Destroy(gameObject);
        }
    }
}