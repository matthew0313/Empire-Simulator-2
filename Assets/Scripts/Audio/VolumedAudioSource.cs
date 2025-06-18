using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class VolumedAudioSource : MonoBehaviour
{
    float m_volume = 1.0f;
    public float volume
    {
        get => m_volume;
        set
        {
            if (m_volume != value)
            {
                m_volume = value;
                OnVolumeChange();
            }
        }
    }
    public AudioSource source { get; private set; }
    private void OnEnable()
    {
        if(source == null) source = GetComponent<AudioSource>();
        Settings.onVolumeChange += OnVolumeChange;
        OnVolumeChange();
    }
    private void OnDisable()
    {
        Settings.onVolumeChange -= OnVolumeChange;
    }
    void OnVolumeChange()
    {
        source.volume = Settings.volume * volume;
    }
}