using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIAudioSlider : MonoBehaviour
{
    Slider slider;

    [System.Serializable]
    public enum MixerGroup
    {
        Master,
        Music,
        Effects,
        UI
    }
    public MixerGroup mixerGroup;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(UpdateMixerGroupVolume);
    }

    private void OnEnable()
    {
        var audioMixer = GameManager.Instance.settings.audioMixer;

        float value = 0f;
        audioMixer.GetFloat(GetMixerName(), out value);
        SetSliderValue(value);
    }

    public string GetMixerName()
    {
        switch (mixerGroup)
        {
            case MixerGroup.Music:
                return "MusicVolume";
            case MixerGroup.Effects:
                return "EffectsVolume";
            case MixerGroup.UI:
                return "UIVolume";
            default:
                return "MasterVolume";
        }
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void UpdateMixerGroupVolume(float value)
    {
        var audioMixer = GameManager.Instance.settings.audioMixer;
        audioMixer.SetFloat(GetMixerName(), value);
    }
}
