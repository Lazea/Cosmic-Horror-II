using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffectsController : MonoBehaviour
{
    private int prevIndex;
    private int i;

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

    // Components
    PlayerCharacterController pc;
    TerrainTextureDetector terrainTextureDetector;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerCharacterController>();
        terrainTextureDetector = new TerrainTextureDetector();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        float randPitch = Random.Range(.87f, 1.13f);
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

        float randPitch = Random.Range(.9f, 1.1f);
        footStepsAudioSource.pitch = randPitch;
        footStepsAudioSource.PlayOneShot(clip, .7f);
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
