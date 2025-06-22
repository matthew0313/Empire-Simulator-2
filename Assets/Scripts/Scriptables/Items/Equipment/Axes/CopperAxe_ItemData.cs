using UnityEngine;

[CreateAssetMenu(fileName = "Copper Axe", menuName = "Scriptables/Item/Equipments/Axes/CopperAxe")]
public class CopperAxe_ItemData : AxeData
{
    [SerializeField] CopperAxe axe;
    public override Equipment Create() => Instantiate(axe).Set(this);
}