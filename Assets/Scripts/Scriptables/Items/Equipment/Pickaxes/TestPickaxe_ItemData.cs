using UnityEngine;

[CreateAssetMenu(fileName = "TestPickaxe", menuName = "Scriptables/Item/Equipments/Pickaxes/TestPickaxe")]
public class TestPickaxe_ItemData : PickaxeData
{
    [SerializeField] TestPickaxe pickaxe;
    public override Equipment Create() => Instantiate(pickaxe).Set(this);
}