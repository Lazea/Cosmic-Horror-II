using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeIntesnity = 1f;
    public float shakeTime = 0.2f;

    float t;
    CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    // Start is called before the first frame update
    void Start()
    {
        
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
        var multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = shakeIntesnity;
        t = shakeTime;
    }

    [ContextMenu("Stop Camera Shake")]
    public void StopCameraShake()
    {
        var multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = 0f;
        t = 0f;
    }
}
