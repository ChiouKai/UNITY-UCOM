using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

 
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string title)
    {        
        Sound s = Array.Find(sounds, sound => sound.title == title);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + title + "not found.");
            return;
        }
        s.source.Play();
    }
}
