using UnityEngine;

[CreateAssetMenu(fileName = "TestPickaxe", menuName = "Scriptables/Item/Equipments/Sickles/TestSickle")]
public class TestSickle_ItemData : SickleData
{
    [SerializeField] TestSickle sickle;
    public override Equipment Create() => Instantiate(sickle).Set(this);
}