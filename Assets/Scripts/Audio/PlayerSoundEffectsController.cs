using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffectsController : MonoBehaviour
{
    private int prevIndex;
    private int i;

    [Header("Footsteps")]
    public FootStepClip defaultFootStepClips;
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

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWalkFootstep()
    {
        Debug.Log("Walk Step");
        PlayFootstep(true);
    }

    public void PlayRunFootstep()
    {
        Debug.Log("Run Step");
        PlayFootstep(false);
    }

    void PlayFootstep(bool isWalk)
    {
        var mat = GetGroundMaterial();

        AudioClip clip = null;
        foreach (var f in footStepClips)
        {
            if (f.material == mat)
            {
                if (isWalk)
                {
                    do
                    {
                        i = Random.Range(0, f.walkClips.Length);
                    }
                    while (i == prevIndex);

                    prevIndex = i;

                    clip = f.walkClips[i];
                }
                else
                {
                    do
                    {
                        i = Random.Range(0, f.runClips.Length);
                    }
                    while (i == prevIndex);

                    prevIndex = i;

                    clip = f.runClips[i];
                }
                break;
            }
        }

        if(clip == null)
        {
            if (isWalk)
            {
                do
                {
                    i = Random.Range(0, defaultFootStepClips.walkClips.Length);
                }
                while (i == prevIndex);

                prevIndex = i;

                clip = defaultFootStepClips.walkClips[i];
            }
            else
            {
                do
                {
                    i = Random.Range(0, defaultFootStepClips.runClips.Length);
                }
                while (i == prevIndex);

                prevIndex = i;

                clip = defaultFootStepClips.runClips[i];
            }
        }
        float randPitch = Random.Range(.87f, 1.13f);
        footStepsAudioSource.pitch = randPitch;

        float randVol = Random.Range(.15f, .2f);

        footStepsAudioSource.PlayOneShot(clip, randVol);
    }

    public void PlayLanding()
    {
        var mat = GetGroundMaterial();
        AudioClip clip = null;
        foreach (var f in footStepClips)
        {
            if (f.material == mat)
            {
                clip = f.landingClip;
                break;
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

    Material GetGroundMaterial()
    {
        var hit = pc.GroundHitGround;
        if (hit.collider != null && hit.collider.tag != "Terrain")
        {
            return hit.collider.GetComponent<MeshRenderer>().
                sharedMaterial;
        }

        return null;
    }
}
