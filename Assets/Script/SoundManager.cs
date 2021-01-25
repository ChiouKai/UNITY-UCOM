using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{    
    public Sound[] sounds;    
    public Slider Volume;    

    // Awake() is called right before it
    void Awake()
    {
       foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.Loop;
        }        
    }       
    
    void Start()
    {   
        Play("Theme_Scene2_Vigilo Confido");                
    }

    //void Update()
    //{
    //    Volume.value = sounds[0].volume;
    //}

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
    public void playVolume()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume*Volume.value;
        }
    }
}

