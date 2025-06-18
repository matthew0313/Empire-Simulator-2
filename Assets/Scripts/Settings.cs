using System;
using UnityEngine;

public static class Settings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        m_language = Language.English;
        m_cameraMoveSpeed = 10.0f;
        m_cameraFlySpeed = 10.0f;
        m_cameraSensitivity = 1.0f;
        m_xRotRange = 75.0f;
        m_yReverse = true;
        m_volume = 1.0f;
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


    static float m_cameraMoveSpeed, m_cameraFlySpeed, m_cameraSensitivity, m_xRotRange;
    static bool m_yReverse;
    public static float cameraMoveSpeed
    {
        get => m_cameraMoveSpeed;
        set
        {
            if(m_cameraMoveSpeed != value)
            {
                m_cameraMoveSpeed = value;
                onCameraMoveSpeedChange?.Invoke();
            }
        }
    }
    public static Action onCameraMoveSpeedChange;
    public static float cameraFlySpeed
    {
        get => m_cameraFlySpeed;
        set
        {
            if(m_cameraFlySpeed != value)
            {
                m_cameraFlySpeed = value;
                onCameraFlySpeedChange?.Invoke();
            }
        }
    }
    public static Action onCameraFlySpeedChange;
    public static float cameraSensitivity
    {
        get => m_cameraSensitivity;
        set
        {
            if(m_cameraSensitivity != value)
            {
                m_cameraSensitivity = value;
                onCameraSensitivityChange?.Invoke();
            }
        }
    }
    public static Action onCameraSensitivityChange;
    public static float xRotRange
    {
        get => m_xRotRange;
        set
        {
            if (m_xRotRange != value)
            {
                m_xRotRange = value;
                onxRotRangeChange?.Invoke();
            }
        }
    }
    public static Action onxRotRangeChange;

    public static bool yReverse
    {
        get => m_yReverse;
        set
        {
            if (m_yReverse != value)
            {
                m_yReverse = value;
                onYReverseChange?.Invoke();
            }
        }
    }
    public static Action onYReverseChange;

    static float m_volume;
    public static float volume
    {
        get => m_volume;
        set
        {
            if (m_volume != value)
            {
                m_volume = value;
                onVolumeChange?.Invoke();
            }
        }
    }
    public static Action onVolumeChange;
}