using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public Dictionary<string, Sound> HSound= new Dictionary<string, Sound>();
    public Sound[] sounds;    
    public Slider Volume;
    Queue<AudioSource> ASs;
    // Awake() is called right before it
    void Awake()
    {
        ASs= new Queue<AudioSource>();
        foreach (Sound s in sounds)
        {
            if (s.clip != null)
            {
                HSound.Add(s.title, s);
            }
        }
    }
    private void Start()
    {
        List<AudioSource> ass=new List<AudioSource>();
        GetComponents<AudioSource>(ass);
        foreach(var a in ass)
        {
            ASs.Enqueue(a);
        }

    }


    public void Play (string title)
    {
        Sound s;
        if (HSound.TryGetValue(title, out s))
        {
            AudioSource a = ASs.Dequeue();
            if (a.isPlaying)
            {
                ASs.Enqueue(a);
                a = gameObject.AddComponent<AudioSource>();
            }
            ASs.Enqueue(a);
            a.clip = s.clip;
            a.volume = s.volume* Volume.value;
            a.pitch = s.pitch;
            a.loop = s.Loop;
            a.Play();
        }

    }        
    public void playVolume()
    {
        foreach (var a in ASs)
        {
            Sound s;
            if(HSound.TryGetValue(a.clip.ToString(),out s))
            {
                a.volume = s.volume * Volume.value;
            }
        }
    }
    public void PlayButton()
    {
        Play("Test");
    }
}

