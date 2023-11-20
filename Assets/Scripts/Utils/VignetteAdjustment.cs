using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteAdjustment : MonoBehaviour
{
    [Header("Vignette Color")]
    public float vignetteColorSmooth = 0.25f;
    public float intensityShift = 0.2f;
    public float intensityAmplitude = 0.5f;
    public Color healedVignetteColor = Color.black;
    public Color hurtVignetteColor = Color.red;
    Color targetVignetteColor;

    [Header("Vignette Intensity")]
    public float frequency;
    float t;
    bool pulsate;

    // Components
    Volume volume;
    Vignette vignette;

    void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        vignette.color.value = Color.Lerp(
            vignette.color.value,
            targetVignetteColor,
            vignetteColorSmooth);

        if(pulsate)
        {
            t += frequency * Time.deltaTime;
            float _a = intensityAmplitude * Mathf.Sin(t) + intensityShift;
            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                _a,
                0.25f);
        }
        else
        {
            t = 0f;
            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                0.5f,
                0.25f);
        }
    }

    public void SetVignetteHurtColor()
    {
        Debug.LogFormat("Set hurt vignette color");
        targetVignetteColor = hurtVignetteColor;
    }

    public void SetVignetteHealedColor()
    {
        Debug.LogFormat("Set healed vignette color");
        targetVignetteColor = healedVignetteColor;
    }

    public void PlayVignettePulse()
    {
        Debug.LogFormat("Play vignette pulse");
        pulsate = true;
    }

    public void StopVignettePulse()
    {
        Debug.LogFormat("Stop vignette pulse");
        pulsate = false;
    }
}
