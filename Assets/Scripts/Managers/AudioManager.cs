using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioManager() => Instance = this;

    readonly Pooler<VolumedAudioSource> audioSourcePool = new(() => new GameObject().AddComponent<VolumedAudioSource>());
    readonly List<VolumedAudioSource> playing = new();
    public VolumedAudioSource PlaySound(AudioClip clip, float volume = 1.0f)
    {
        var tmp = audioSourcePool.GetObject();
        tmp.volume = volume;
        tmp.source.clip = clip;

        playing.Add(tmp);
        tmp.source.Play();

        return tmp;
    }
    public VolumedAudioSource PlaySound(AudioVolumePair pair) => PlaySound(pair.clip, pair.volume);
    readonly List<VolumedAudioSource> removeQueue = new();
    private void Update()
    {
        foreach(var i in playing)
        {
            if (!i.source.isPlaying) removeQueue.Add(i);
        }
        foreach(var i in removeQueue) playing.Remove(i); removeQueue.Clear();
    }
    public void StopSound(VolumedAudioSource source)
    {
        source.source.Stop();
        removeQueue.Add(source);
    }
}
[System.Serializable]
public struct AudioVolumePair
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;
}