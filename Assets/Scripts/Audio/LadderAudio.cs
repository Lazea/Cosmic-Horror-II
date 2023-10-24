using UnityEngine;

public class LadderAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource interactLadderSrc;
    public AudioSource climbLadderSrc;

    [Header("Volume Control")]
    public float interactVolume;
    public float climbVolume;

    public void LadderInteractAudio()
    {
        interactLadderSrc.pitch = Random.Range(0.9f, 1.1f);
        interactLadderSrc.PlayOneShot(interactLadderSrc.clip, interactVolume);
    }

    public void OnClimb()
    {
        climbLadderSrc.Play();
    }

    public void OnDismount()
    {
        interactLadderSrc.pitch = Random.Range(0.9f, 1.1f);
        interactLadderSrc.PlayOneShot(interactLadderSrc.clip, interactVolume);
        Invoke(nameof(StopSound), .5f);
    }

    public void StopClimbing()
    {
        climbLadderSrc.Pause();
    }

    void StopSound()
    {
        climbLadderSrc.Stop();
    }
}