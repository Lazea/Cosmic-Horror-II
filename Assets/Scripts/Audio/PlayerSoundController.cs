using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public bool isBreathing = false;
    public bool isHealed = false;

    int PrevIndex;

    [Header("Audio Clips")]
    public AudioClip[] attackGrunts;
    public AudioClip[] damageGrunts;
    public AudioClip[] climbGrunts;
    public AudioClip[] landingGrunts;
    public AudioClip[] breathing = new AudioClip[2];
    public AudioClip[] shortbreath = new AudioClip[2];

    [Header("Pitch Control")]
    public float minPitch;
    public float maxPitch;

    [Header("Volume Control")]
    public float attackGruntVol;
    public float damageGruntVol;
    public float climbGruntVol;
    public float landingGruntVol;
    public float breathingVol;
    public float calmVol;

    [Header("Probability Control: 0 - 1")]
    public float attackProb;
    public float damageProb;
    public float climbProb;
    public float landingProb;

    [Header("Audio Source")]
    public AudioSource src;
    public AudioSource breathsrc;

    private void Start()
    {
        src.enabled = false;
        Invoke(nameof(EnableSrc), 20f);
    }

    void EnableSrc()
    {
        src.enabled = true; 
    }

    public void AttackGrunt()
    {
        float prob = Random.Range(0f, 1f);

        if(prob < attackProb)
        {
            int Index;

            do
            {
                Index = Random.Range(0, attackGrunts.Length);
            }
            while (Index == PrevIndex);

            float randVol = Random.Range(attackGruntVol - .1f, attackGruntVol + .1f);

            PrevIndex = Index;

            PlayAudio(attackGrunts[Index]);
        }    
    }

    public void DamageGrunt()
    {
        float prob = Random.Range(0f, 1f);

        if (prob < damageProb)
        {
            int Index;

            do
            {
                Index = Random.Range(0, damageGrunts.Length);
            }
            while (Index == PrevIndex);

            float randVol = Random.Range(damageGruntVol - .1f, damageGruntVol + .1f);

            PrevIndex = Index;

            PlayAudio(damageGrunts[Index]);
        }
    }

    public void ClimbGrunt()
    {
        float prob = Random.Range(0f, 1f);

        if (prob < climbProb)
        {
            int Index = Random.Range(0, climbGrunts.Length);
            float randVol = Random.Range(climbGruntVol - .1f, climbGruntVol + .1f);

            PlayAudio(climbGrunts[Index]);
        }
    }

    public void LandingGrunt()
    {
        float prob = Random.Range(0f, 1f);

        if (prob < landingProb)
        {
            int Index = Random.Range(0, landingGrunts.Length);
            float randVol = Random.Range(landingGruntVol - .1f, landingGruntVol + .1f);

            PlayAudio(landingGrunts[Index]);
        }
    }

    public void Breath()
    {
        if (isHealed)
        {
            isBreathing = true;
            HandleBreathing();
        }
    }

    void HandleBreathing()
    {
        if (isBreathing)
        {
            int Index;

            do
            {
                Index = Random.Range(0, 2);
            }
            while (Index == PrevIndex);

            float randPitch = Random.Range(.9f, 1.05f);
            breathsrc.volume = breathingVol;
            breathsrc.pitch = randPitch;
            breathsrc.clip = breathing[Index];
            PrevIndex = Index;

            breathsrc.Play();
        }
    }

    public void Calm()
    {
        isBreathing = false;
        breathsrc.Stop();

        int Index = Random.Range(0, 2);
        float randPitch = Random.Range(.9f, 1.05f);
        src.pitch = randPitch;
        src.PlayOneShot(shortbreath[Index], calmVol);
    }

    void PlayAudio(AudioClip clip)
    {
        if(src.isPlaying == false && src != null)
        {
            float randPitch = Random.Range(minPitch, maxPitch);
            src.pitch = randPitch;
            src.clip = clip;

            src.Play();

            breathsrc.Stop();
            Invoke(nameof(HandleBreathing), clip.length);
        }
    }
}
