using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeIntesnity = 1f;
    public float shakeTime = 0.2f;
    public float idleShaleIntensity = 1f;

    float t;
    CinemachineBasicMultiChannelPerlin multiChannelPerlin;
    public NoiseSettings idleNoiseSettings;
    public NoiseSettings impactNoiseSettings;

    // Start is called before the first frame update
    void Start()
    {
        multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StopCameraShake();
    }

    // Update is called once per frame
    void Update()
    {
        if(t > 0f)
        {
            t -= Time.deltaTime;
            if(t <= 0f)
            {
                StopCameraShake();
            }
        }
    }

    [ContextMenu("Shake Camera")]
    public void ShakeCamera()
    {
        multiChannelPerlin.m_NoiseProfile = impactNoiseSettings;
        multiChannelPerlin.m_AmplitudeGain = shakeIntesnity;
        t = shakeTime;
    }

    [ContextMenu("Stop Camera Shake")]
    public void StopCameraShake()
    {
        multiChannelPerlin.m_NoiseProfile = idleNoiseSettings;
        multiChannelPerlin.m_AmplitudeGain = idleShaleIntensity;
        t = 0f;
    }
}
