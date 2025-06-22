using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildTabIngredientElement : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemText;
    public void Set(ItemData item, int count)
    {
        itemImage.sprite = item.itemIcon;
        itemText.text = $"{item.itemName.text} x{count}";
    }
}