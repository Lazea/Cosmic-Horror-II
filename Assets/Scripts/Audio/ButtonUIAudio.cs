using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonUIAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioClip clickClip;
    public AudioClip enterClip;

    public void OnPointerDown(PointerEventData eventData)
    {
        UIAudioManager.Instance.PlayUISound(clickClip, .4f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIAudioManager.Instance.PlayUISound(enterClip, .15f);
    }
}
