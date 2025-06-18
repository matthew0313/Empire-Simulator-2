using UnityEngine;

[CreateAssetMenu(fileName = "CopperPickaxe", menuName = "Scriptables/Item/Equipments/Pickaxes/CopperPickaxe")]
public class CopperPickaxe_ItemData : PickaxeData
{
    [SerializeField] CopperPickaxe pickaxe;
    public override Equipment Create() => Instantiate(pickaxe).Set(this);
}