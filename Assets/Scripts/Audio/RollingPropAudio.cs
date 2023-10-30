using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingPropAudio : MonoBehaviour
{
    public AudioSource src;
    public AudioSource brksrc;
    float initialVol;

    private void Start()
    {
        initialVol = src.volume;
        src.enabled = false;
        Invoke(nameof(EnableSRC), 4f);

    }

    public void SourceManipulation(AudioClip clip)
    {
        src.clip = clip;
        
        float randPitch = Random.Range(.93f, 1.07f);
        float randVol = Random.Range((initialVol - .1f), (initialVol + .1f));
        float randTime = Random.Range(0f, .1f);

        src.pitch = randPitch;
        src.volume = randVol;
        src.time = randTime;
    }

    public void BreakBehavior(AudioClip clip)
    {
        src.clip = clip;

        if(src.isPlaying == false)
        {
            src.Play();        
        }
    }

    void EnableSRC()
    {
        src.enabled = true;
    }
}
