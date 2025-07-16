using System.Collections.Generic;
using UnityEngine;

public class DefaultUI : TabOpener
{
    [SerializeField] InventoryTab m_inventoryTab;
    [SerializeField] BuildTab m_buildTab;
    [SerializeField] RectTransform buttons;
    public InventoryTab inventoryTab => m_inventoryTab;
    public BuildTab buildTab => m_buildTab;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        CloseTab();
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTab(); return;
        }
    }
}