using System;
using UnityEngine;

public static class Settings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        m_language = Language.English;
    }
    static Language m_language;
    public static Language language
    {
        get => m_language;
        set
        {
            if (m_language != value)
            {
                m_language = value;
                onLanguageChange?.Invoke();
            }
        }
    }
    public static Action onLanguageChange;
}