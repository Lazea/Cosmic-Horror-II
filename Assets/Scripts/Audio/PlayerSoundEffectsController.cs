using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSoundEffectsController : MonoBehaviour
{
    private int prevIndex;
    private int i;
    private int LightAttackIndex = 0;
    private int ContactIndex = 0;

    public AudioClip ClimbingAudio;
    public AudioClip PickKey;
    public AudioClip PickHealth;
    public AudioClip Throw;
    public AudioClip[] DamagePlayerAudio = new AudioClip[3];

    [Header("Prop Audio")]
    public AudioClip GeneralPickup;
    public AudioClip MetalLightPickup;
    public AudioClip MetalHeavyPickup;
    public AudioClip WoodLightPickup;
    public AudioClip WoodHeavyPickup;

    [Header("Attack Audio")]
    public AudioClip[] LightAttacks = new AudioClip[2];
    public AudioClip HeavyAttack;
    public AudioClip[] ContactGeneric = new AudioClip[2];
    public AudioClip[] ContactMetal = new AudioClip[2];
    public AudioClip[] ContactWood = new AudioClip[2];


    public float keyVol;
    public float healthVol;
    public float damageVol;

    [Header("Footsteps")]
    public FootStepClip defaultFootStepClips;
    public FootStepClip gravelFootStepClips;
    public FootStepClip dirtFootStepClips;
    public FootStepClip grassFootStepClips;
    public FootStepClip[] footStepClips;
    [System.Serializable]
    public struct FootStepClip
    {
        public AudioClip[] walkClips;
        public AudioClip[] runClips;
        public AudioClip landingClip;
        public Material material;
    }
    public AudioSource footStepsAudioSource;

    public AudioSource oneShotSource;

    public AudioSource reverblessSrc;

    public AudioSource propSrc;

    // Components
    PlayerCharacterController pc;
    TerrainTextureDetector terrainTextureDetector;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerCharacterController>();
        terrainTextureDetector = new TerrainTextureDetector();
    }

    /*
    public void AttackAudio(BaseProp bProp)
    {
        switch(bProp.propWeight)
        {
            case PropWeight.Light:
                AttackLight();
                break;

            case PropWeight.Heavy:
                PlayAttackAudio(HeavyAttack);
                break;
        }
    }
    */

    public void AttackLight()
    {
        LightAttackIndex = (LightAttackIndex + 1) & 2;

        PlayAttackAudio(LightAttacks[LightAttackIndex]);
    }

    void PlayAttackAudio(AudioClip clip)
    {
        propSrc.Stop();
        float randPitch = Random.Range(.7f, .8f);
        float randVol = Random.Range(.2f, .3f);
        propSrc.pitch = randPitch;

        propSrc.PlayOneShot(clip, randVol);
    }



    public void PickUpAudio(BaseProp bProp)
    {
        switch (bProp.propWeight, bProp.propMaterial)
        {
            case (PropWeight.Light, PropMaterial.Metal):
                PlayPickupAudio(MetalLightPickup);
                break;

            case (PropWeight.Light, PropMaterial.Wood):
                PlayPickupAudio(WoodLightPickup);
                break;

            case (PropWeight.Light, PropMaterial.Glass):
                PlayPickupAudio(GeneralPickup);
                break;

            case (PropWeight.Light, PropMaterial.Other):
                PlayPickupAudio(GeneralPickup);
                break;

            case(PropWeight.Heavy, PropMaterial.Metal):
                PlayPickupAudio(MetalHeavyPickup);
                break;

            case (PropWeight.Heavy, PropMaterial.Wood):
                PlayPickupAudio(WoodHeavyPickup);
                break;

            case (PropWeight.Heavy, PropMaterial.Glass):
                PlayPickupAudio(GeneralPickup);
                break;

            case (PropWeight.Heavy, PropMaterial.Other):
                PlayPickupAudio(GeneralPickup);
                break;
        }
    }

    public void HitContactAudio(BaseProp bProp)
    {
        switch(bProp.propWeight, bProp.propMaterial)
        {
            case (PropWeight.Light, PropMaterial.Metal):
                PlayContactAudio(ContactGeneric);
                break;

            case (PropWeight.Light, PropMaterial.Wood):
                PlayContactAudio(ContactGeneric);
                break;

            case (PropWeight.Light, PropMaterial.Glass):
                PlayContactAudio(ContactGeneric);
                break;

            case (PropWeight.Light, PropMaterial.Other):
                PlayContactAudio(ContactGeneric);
                break;

            case (PropWeight.Heavy, PropMaterial.Metal):
                PlayContactAudio(ContactMetal);
                break;

            case (PropWeight.Heavy, PropMaterial.Wood):
                PlayContactAudio(ContactWood);
                break;

            case (PropWeight.Heavy, PropMaterial.Glass):
                PlayContactAudio(ContactGeneric);
                break;

            case (PropWeight.Heavy, PropMaterial.Other):
                PlayContactAudio(ContactGeneric);
                break;
        }
    }

    void PlayContactAudio(AudioClip[] clip)
    {
        ContactIndex = (ContactIndex + 1) % 2;
        float randPitch = Random.Range(.9f, 1.1f);
        float randVol = Random.Range(.2f, .3f);
        propSrc.pitch = randPitch;

        propSrc.PlayOneShot(clip[ContactIndex], randVol);
    }

    /*
    void HandlePickupLight(BaseProp bProp)
    {
        switch(bProp.propMaterial)
        {
            case PropMaterial.Metal:
                //light metal pickup
                PlayPickupAudio(MetalLightPickup);

                break;
            case PropMaterial.Wood:
                //light wood pickup
                PlayPickupAudio(WoodLightPickup);

                break;
            case PropMaterial.Glass:
                //generic pickup
                PlayPickupAudio(GeneralPickup);

                break;
            case PropMaterial.Other:
                //generic pickup
                PlayPickupAudio(GeneralPickup);

                break;
        }
    }

    void HandlePickupHeavy(BaseProp bProp)
    {
        switch (bProp.propMaterial)
        {
            case PropMaterial.Metal:
                //heavy metal pickup
                PlayPickupAudio(MetalHeavyPickup);

                break;
            case PropMaterial.Wood:
                //heavy wood pickup
                PlayPickupAudio(WoodHeavyPickup);

                break;
            case PropMaterial.Glass:
                //generic pickup
                PlayPickupAudio(GeneralPickup);

                break;
            case PropMaterial.Other:
                //generic pickup
                PlayPickupAudio(GeneralPickup);

                break;
        }
    }

    */

    void PlayPickupAudio(AudioClip clip)
    {
        float randPitch = Random.Range(.9f, 1.1f);
        float randVol = Random.Range(.7f, .8f);
        propSrc.pitch = randPitch;

        propSrc.PlayOneShot(clip, randVol);
    }

    public void DropAudio()
    {
        float randPitch = Random.Range(.7f, .8f);
        float randVol = Random.Range(.05f, .15f);
        propSrc.pitch = randPitch;

        propSrc.PlayOneShot(Throw, randVol);
    }

    public void ThrowAudio()
    {
        float randPitch = Random.Range(.9f, 1.1f);
        float randVol = Random.Range(.2f, .3f);
        propSrc.pitch = randPitch;

        propSrc.PlayOneShot(Throw, randVol);
    }

    public void PlayWalkFootstep()
    {
        PlayFootstep(true);
    }

    public void PlayRunFootstep()
    {
        PlayFootstep(false);
    }

    void PlayFootstep(bool isWalk)
    {
        AudioClip clip = null;

        if(pc.GroundHit.collider != null)
        {
            if (pc.GroundHit.collider.GetComponent<Terrain>() != null)
            {
                clip = GetClipFromTerrainTexture(isWalk);
            }
            else
            {
                clip = GetClipFromGroundMaterial(isWalk);
            }
        }

        if (clip == null)
            clip = GetRandomClip(defaultFootStepClips, isWalk);

        float randPitch = Random.Range(.83f, 1f);
        footStepsAudioSource.pitch = randPitch;

        float randVol = Random.Range(.15f, .2f);

        footStepsAudioSource.PlayOneShot(clip, randVol);
    }

    public void PlayLanding()
    {
        AudioClip clip = null;

        if (pc.GroundHit.collider != null)
        {
            if (pc.GroundHit.collider.GetComponent<Terrain>() != null)
            {
                int terrainTextureIndex = terrainTextureDetector.
                    GetActiveTerrainTextureIdx(transform.position);

                switch (terrainTextureIndex)
                {
                    case 0:
                        clip = gravelFootStepClips.landingClip;
                        break;
                    case 1:
                        clip = grassFootStepClips.landingClip;
                        break;
                    case 2:
                    case 3:
                        clip = dirtFootStepClips.landingClip;
                        break;
                }
            }
            else
            {
                var mat = GetGroundMaterial();
                foreach (var f in footStepClips)
                {
                    if (f.material == mat)
                    {
                        clip = f.landingClip;
                        break;
                    }
                }
            }
        }

        if(clip == null)
        {
            clip = defaultFootStepClips.landingClip;
        }

        float randPitch = Random.Range(.8f, 1f);
        footStepsAudioSource.pitch = randPitch;
        footStepsAudioSource.PlayOneShot(clip, .15f);
    }

    public void PlayClimbing()
    {
        float randPitch = Random.Range(.8f, .9f);
        footStepsAudioSource.pitch = randPitch;
        footStepsAudioSource.PlayOneShot(ClimbingAudio, .07f);
    }

    //Player Take Damage
    public void DamagePlayer()
    {
        int RandIndex = Random.Range(0, 2);
        float randPitch = Random.Range(.95f, 1.05f);
        reverblessSrc.PlayOneShot(DamagePlayerAudio[RandIndex], damageVol);

    }

    //Player Pickup Key
    public void KeyAudio()
    {
        oneShotSource.PlayOneShot(PickKey, keyVol);
    }

    //Player Pickup Bandage
    public void HealthAudio()
    {
        oneShotSource.PlayOneShot(PickHealth, healthVol);
    }


    AudioClip GetClipFromGroundMaterial(bool isWalk)
    {
        var mat = GetGroundMaterial();

        AudioClip clip = null;
        foreach (var f in footStepClips)
        {
            if (f.material == mat)
            {
                clip = GetRandomClip(f, isWalk);
                break;
            }
        }

        return clip;
    }

    AudioClip GetClipFromTerrainTexture(bool isWalk)
    {
        int terrainTextureIndex = terrainTextureDetector.
            GetActiveTerrainTextureIdx(transform.position);

        AudioClip clip = null;

        switch (terrainTextureIndex)
        {
            case 0:
                clip = GetRandomClip(gravelFootStepClips, isWalk);
                break;
            case 1:
                clip = GetRandomClip(grassFootStepClips, isWalk);
                break;
            case 2:
            case 3:
                clip = GetRandomClip(dirtFootStepClips, isWalk);
                break;
        }

        return clip;
    }

    AudioClip GetRandomClip(FootStepClip footStepClip, bool isWalk)
    {
        AudioClip[] clips = (isWalk) ? footStepClip.walkClips : footStepClip.runClips;
        do
        {
            i = Random.Range(0, clips.Length);
        }
        while (i == prevIndex);

        prevIndex = i;
        return clips[i];
    }

    Material GetGroundMaterial()
    {
        var hit = pc.GroundHit;
        if (hit.collider != null && hit.collider.tag != "Terrain")
        {
            return hit.collider.GetComponent<MeshRenderer>().
                sharedMaterial;
        }

        return null;
    }
}
