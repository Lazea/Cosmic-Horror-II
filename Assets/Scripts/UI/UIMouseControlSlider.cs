using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIMouseControlSlider : MonoBehaviour
{
    Slider slider;

    [System.Serializable]
    public enum SensitivityAxis
    {
        X,
        Y
    }
    public SensitivityAxis sensitivityAxis;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(UpdateSensitivitySettings);
    }

    private void OnEnable()
    {
        var controlSettings = GameManager.Instance.settings.controlSettings;

        if(sensitivityAxis == SensitivityAxis.X)
        {
            SetSliderValue(controlSettings.xSensitivity);
        }
        else
        {
            SetSliderValue(controlSettings.ySensitivity);
        }
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void UpdateSensitivitySettings(float value)
    {
        var controlSettings = GameManager.Instance.settings.controlSettings;

        if (sensitivityAxis == SensitivityAxis.X)
        {
            if (controlSettings.xSensitivity != value)
                GameManager.Instance.settings.controlSettings.xSensitivity = value;
        }
        else
        {
            if (controlSettings.ySensitivity != value)
                GameManager.Instance.settings.controlSettings.ySensitivity = value;
        }
    }
}
