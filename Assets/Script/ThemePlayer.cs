using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ThemePlayer : MonoBehaviour
{
    AudioSource aSource;
    public AudioClip[] Themes;
    public Slider Volume;

    void Awake()
    {
        aSource = GetComponent<AudioSource>();
        aSource.GetComponent<AudioSource>().clip = Themes[0];       
        this.GetComponent<AudioSource>().Play();
        aSource.loop = true;
    }

    public void StopThemes(int i)
    {
        aSource.clip = Themes[i];
        this.GetComponent<AudioSource>().Stop();
    }
    
    public void PlayThemes(int i)
    {
        aSource.clip = Themes[i];        
        aSource.loop = true;
        this.GetComponent<AudioSource>().Play();
    }

    public void PlayVolume()
    {
        aSource.volume = Volume.value;
    }

    //public void StopMainTheme()
    //{
    //    if (MainThemePlaying) 
    //}        

    //public void PlayFailureTheme()
    //{        
    //    aSource.GetComponent<AudioSource>().clip = Themes[1];
    //    this.GetComponent<AudioSource>().Play();
    //}

    //public void PlayVictoryTheme()
    //{
    //    aSource.GetComponent<AudioSource>().clip = Themes[2];
    //    this.GetComponent<AudioSource>().Play();
    //}

}
