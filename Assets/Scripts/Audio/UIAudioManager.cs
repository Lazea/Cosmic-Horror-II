using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIAudioManager : Singleton<UIAudioManager>
{
    AudioSource src;

    private void Start()
    {
        src = GetComponent<AudioSource>();    
    }

    public void PlayUISound(AudioClip clip, float vol)
    {
        src.PlayOneShot(clip, vol);
    }
}
