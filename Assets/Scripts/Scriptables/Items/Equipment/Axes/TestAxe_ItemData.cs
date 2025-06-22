using UnityEngine;

[CreateAssetMenu(fileName = "TestAxe", menuName = "Scriptables/Item/Equipments/Axes/TestAxe")]
public class TestAxe_ItemData : AxeData
{
    [SerializeField] TestAxe axe;
    public override Equipment Create() => Instantiate(axe).Set(this);
}