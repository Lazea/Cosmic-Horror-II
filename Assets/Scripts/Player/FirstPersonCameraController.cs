using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    Vector2 mouseDelta;
    float yaw = 0f;
    float pitch = 0f;

    public bool lockXRot;
    public bool lockYRot;

    ControlSettings controlSettings
    {
        get { return GameManager.Instance.settings.controlSettings; }
    }

    void Update()
    {
        if(!lockXRot)
        {
            float mouseX = mouseDelta.x * controlSettings.ySensitivity * 0.75f * Time.timeScale;
            yaw += mouseX;// * Time.deltaTime;
        }
        else
        {
            yaw = 0f;
        }

        if (!lockYRot)
        {
            float mouseY = mouseDelta.y * controlSettings.xSensitivity *
                (controlSettings.yInverted ? 1 : -1) * 0.75f * Time.timeScale;

            pitch += mouseY;// * Time.deltaTime;
            pitch = Mathf.Clamp(
                pitch,
                -controlSettings.pitchClamp,
                controlSettings.pitchClamp);
        }
        else
        {
            pitch = 0f;
        }

        // Rotate the camera based on pitch and yaw angles
        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    }

    public void GetMouseDelta(Vector2 look)
    {
        mouseDelta = look;
    }
}
