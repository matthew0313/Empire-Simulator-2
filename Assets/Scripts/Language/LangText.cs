using UnityEngine;

[System.Serializable]
public struct LangText
{
    public string en, kr;
    public string text
    {
        get
        {
            switch (Settings.language)
            {
                case Language.English: return en;
                case Language.Korean: return kr;
                default: return en;
            }
        }
    }
    public static LangText operator +(LangText a, LangText b)
    {
        return new()
        {
            en = a.en + b.en,
            kr = a.kr + b.kr
        };
    }
}