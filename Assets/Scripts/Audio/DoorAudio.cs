using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    AudioSource src;

    float minPitch, maxPitch;
    float randPitch;

    [Header("Audio Volumes")]
    public float openVolume;
    public float closeVolume;
    public float lockVolume;
    public float unlockVolume;

    [Header("Audio Clips")]
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip doorLocked;
    public AudioClip doorUnlock;

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        minPitch = .95f;
        maxPitch = 1.05f;
    }

    public void OnDoorSwing()
    {
        if(src.isPlaying == false)
        {
            src.pitch = Random.Range(minPitch, maxPitch);
            src.PlayOneShot(doorOpen, openVolume);
        }
    }

    public void onDoorLocked()
    {
        if(src.isPlaying == false)
        {
            src.PlayOneShot(doorLocked, lockVolume);
        }
    }

    public void onDoorUnlock()
    {
        if (src.isPlaying)
            src.Stop();
        src.PlayOneShot(doorUnlock, unlockVolume);
        //if (src.isPlaying == false)
        //{
        //    src.PlayOneShot(doorUnlock, unlockVolume);
        //}
    }
}
