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
            float mouseX = mouseDelta.x * controlSettings.ySensitivity;
            yaw = transform.localEulerAngles.y + mouseX;
        }
        else
        {
            yaw = 0f;
        }

        if (!lockYRot)
        {
            float mouseY = mouseDelta.y * controlSettings.xSensitivity *
                (controlSettings.yInverted ? 1 : -1);

            pitch += mouseY;
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
