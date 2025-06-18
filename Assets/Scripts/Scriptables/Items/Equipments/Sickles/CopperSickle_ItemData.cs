using UnityEngine;

[CreateAssetMenu(fileName = "TestPickaxe", menuName = "Scriptables/Item/Equipments/Sickles/CopperSickle")]
public class CopperSickle_ItemData : SickleData
{
    [SerializeField] CopperSickle sickle;
    public override Equipment Create() => Instantiate(sickle).Set(this);
}