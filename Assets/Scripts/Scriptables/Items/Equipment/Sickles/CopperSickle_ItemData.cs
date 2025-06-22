using UnityEngine;

[CreateAssetMenu(fileName = "Copper Sickle", menuName = "Scriptables/Item/Equipments/Sickles/CopperSickle")]
public class CopperSickle_ItemData : SickleData
{
    [SerializeField] CopperSickle sickle;
    public override Equipment Create() => Instantiate(sickle).Set(this);
}