using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour, IAudible
{
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
    private SoundManager soundManager;
    private AudioSource currentLoopingSource;

    public void Initialize(string[] soundTypes)
    {
        soundManager = FindObjectOfType<SoundManager>();
        foreach (var soundType in soundTypes)
        {
            CreateAudioSource(soundType);
        }
    }

    private void CreateAudioSource(string key)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.maxDistance = 40f;
        source.rolloffMode = AudioRolloffMode.Linear;
        audioSources[key] = source;
    }

    public void PlaySound(string soundKey, bool isLooping = false)
    {
        if (string.IsNullOrEmpty(soundKey)) return;

        if (audioSources.TryGetValue("footsteps", out AudioSource source))
        {
            source.loop = isLooping;
            soundManager.PlaySound(soundKey, source);
        }
    }

    public void PlayWaypointSound(string soundKey, bool isLooping)
    {
        if (string.IsNullOrEmpty(soundKey)) return;

        StopWaypointSound();

        if (audioSources.TryGetValue("waypoint", out AudioSource source))
        {
            source.loop = isLooping;
            if (soundManager != null)
            {
                AudioClip clip = soundManager.GetAudioClip(soundKey);
                if (clip != null)
                {
                    source.clip = clip;
                    source.Play();
                    currentLoopingSource = source;
                }
            }
        }
    }

    public void StopWaypointSound()
    {
        if (currentLoopingSource != null && currentLoopingSource.isPlaying)
        {
            currentLoopingSource.Stop();
            currentLoopingSource = null;
        }
    }

    public void StopSound(string soundKey)
    {
        if (audioSources.TryGetValue(soundKey, out AudioSource source))
        {
            source.Stop();
        }
    }

    private void OnDestroy()
    {
        StopWaypointSound();
        foreach (var source in audioSources.Values)
        {
            if (source != null)
            {
                source.Stop();
            }
        }
    }
}