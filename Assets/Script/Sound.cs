using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class Sound 
{
    public string title;
    public AudioClip clip;
    public bool Loop;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.3f, 1f)]
    public float pitch;

    [HideInInspector] //Hide so as to public this on the Awake() in Manager
    public AudioSource source;
}
