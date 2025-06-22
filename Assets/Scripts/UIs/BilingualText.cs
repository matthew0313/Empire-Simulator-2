using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BilingualText : MonoBehaviour
{
    [SerializeField] LangText contents;
    TMP_Text origin;
    private void OnEnable()
    {
        Settings.onLanguageChange += OnLanguageChange;
        OnLanguageChange();
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
    }
    void OnLanguageChange()
    {
        if(origin == null) origin = GetComponent<TMP_Text>();
        origin.text = contents.text;
    }
}