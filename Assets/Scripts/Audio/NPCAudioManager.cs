using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAudioManager : MonoBehaviour
{
    int DamageIndex = 0;
    
    [Header("Audio Sources")]
    public AudioSource loopSrc;
    public AudioSource oneShotSrc;

    [Header("Audio Clips (VO)")]
    public AudioClip[] AttackGrunts = new AudioClip[3];
    public AudioClip[] DamageGrunts = new AudioClip[2];
    public AudioClip[] DeathGrunts = new AudioClip[2];

    [Header("Audio Clips (SX)")]
    public AudioClip[] AttackSX = new AudioClip[3];
    public AudioClip[] DamageSX = new AudioClip[3];
    public AudioClip[] ThrowDamageSX = new AudioClip[2];

    [Header("Volume Control")]
    public float minVol;
    public float maxVol;
    public float attackVolVO;
    public float attackVolSX;
    public float damageVolVO;
    public float damageVolSX;
    public float deathVolVO;

    [Header("Pitch Control")]
    public float minPitch;
    public float maxPitch;

    public void IdleAudio()
    {
        //Making sure the loop sounds different from enemy to enemy
        float randPitch = Random.Range(minPitch, maxPitch);
        float randVol = Random.Range(minVol, maxVol);
        float randTime = Random.Range(0f, 4f);
        loopSrc.pitch = randPitch;
        loopSrc.volume = randVol;
        loopSrc.time = randTime;

        loopSrc.Play();
    }

    public void LightAttackAudio()
    {
        oneShotSrc.Stop();
        
        int RandInt = Random.Range(0, 2);
        float randPitch = Random.Range(.9f, 1.1f);
        oneShotSrc.pitch = randPitch;
        oneShotSrc.time = .25f;
        oneShotSrc.volume = attackVolVO;
        oneShotSrc.clip = AttackGrunts[RandInt];
        oneShotSrc.Play();

        oneShotSrc.PlayOneShot(AttackSX[RandInt], attackVolSX);


        //oneShotSrc.PlayOneShot(AttackGrunts[RandInt], attackVolVO);
    }

    public void HeavyAttackAudio()
    {
        oneShotSrc.Stop();

        int RandInt = Random.Range(0, 2);
        float randPitch = Random.Range(.75f, .95f);
        oneShotSrc.pitch = randPitch;
        oneShotSrc.time = .3f;
        oneShotSrc.volume = attackVolVO;
        oneShotSrc.clip = AttackGrunts[RandInt];
        oneShotSrc.Play();

        oneShotSrc.PlayOneShot(AttackSX[RandInt], attackVolSX);


        //oneShotSrc.PlayOneShot(AttackGrunts[RandInt], attackVolVO + .1f);
    }

    public void DamageAudio()
    {
        oneShotSrc.Stop();

        DamageIndex = (DamageIndex + 1) % 2;

        float randPitch = Random.Range(.9f, 1f);
        oneShotSrc.pitch = randPitch;
        oneShotSrc.PlayOneShot(DeathGrunts[DamageIndex], damageVolVO);

        int RandIndex = Random.Range(0, 2);
        float randPitchsx = Random.Range(.8f, .9f);
        oneShotSrc.pitch = randPitchsx;
        oneShotSrc.PlayOneShot(DamageSX[RandIndex], damageVolSX);

        oneShotSrc.PlayOneShot(ThrowDamageSX[DamageIndex], .7f);
    }

    public void DeathAudio()
    {
        oneShotSrc.Stop();
        loopSrc.Stop();
        
        int RandInt = Random.Range(0, 1);
        float randPitch = Random.Range(.9f, 1f);
        oneShotSrc.pitch = randPitch;
        oneShotSrc.PlayOneShot(DeathGrunts[RandInt], deathVolVO);
    }


    public void StopNPC()
    {
        loopSrc.Stop();
    }

}
