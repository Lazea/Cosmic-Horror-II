using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIMouseControlToggle : MonoBehaviour
{
    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        toggle.onValueChanged.AddListener((value) => { UpdateInvertedSettings(value); });
    }

    private void OnEnable()
    {
        var controlSettings = GameManager.Instance.settings.controlSettings;
        SetSliderValue(controlSettings.yInverted);
    }

    public void SetSliderValue(bool isOn)
    {
        toggle.isOn = isOn;
    }

    public void UpdateInvertedSettings(bool isOn)
    {
        var controlSettings = GameManager.Instance.settings.controlSettings;
        if (isOn != controlSettings.yInverted)
        {
            GameManager.Instance.settings.controlSettings.yInverted = isOn;
        }
    }
}
