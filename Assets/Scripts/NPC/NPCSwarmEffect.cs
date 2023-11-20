using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSwarmEffect : MonoBehaviour
{
    public ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        var module = particleSystem.main;
        module.playOnAwake = false;
    }

    public void PlayEffect()
    {
        particleSystem.Play();
    }

    public void StopEffect()
    {
        particleSystem.Stop();
    }
}
