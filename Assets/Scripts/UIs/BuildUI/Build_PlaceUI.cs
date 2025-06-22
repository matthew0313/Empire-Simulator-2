using System.Collections.Generic;
using UnityEngine;

public class Build_PlaceUI : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}