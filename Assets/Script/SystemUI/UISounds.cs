using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UISounds : MonoBehaviour
{
    public AudioSource aSource;
    public AudioClip aMouseOn;
    public AudioClip aMouseDown;
    public AudioClip aQuitGame;
    public Slider Volume;

    public void PlayOnMouseEnter()
    {
        aSource.PlayOneShot(aMouseOn);
    }
    public void PlayOnMouseDown()
    {
        aSource.PlayOneShot(aMouseDown);
    }
    public void PlaySoundQuitGame()
    {
        aSource.PlayOneShot(aQuitGame);
    }
    public void PlayVolume()
    {
        aSource.volume = Volume.value;
    }
}
