using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Camera fpsCam;
    public Sound[] sounds;

    // Awake() is called right before it
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
    
    void Start()
    {   
        Play("Theme_Open_Short epic");                
    }

    public void Play (string title)
    {
        //using System to use Array
        //look into sounds for sound which its "title" is "tile"
        Sound s = Array.Find(sounds, sound => sound.title == title);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + title + "not found.");
            return;
        }
        s.source.Play();                     
    }        
    
    
}

