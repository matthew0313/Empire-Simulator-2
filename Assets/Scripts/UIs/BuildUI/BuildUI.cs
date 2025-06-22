using System.Collections.Generic;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [SerializeField] Build_PlaceUI m_placeUI;
    [SerializeField] Build_DestroyUI m_destroyUI;
    public Build_PlaceUI placeUI => m_placeUI;
    public Build_DestroyUI destroyUI => m_destroyUI;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}