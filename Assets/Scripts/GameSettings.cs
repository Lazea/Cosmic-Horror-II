using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName="SO_GameSettings",
    menuName="ScriptableObjects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public PlayerSettings playerSettings;
    public ControlSettings controlSettings;
}

[System.Serializable]
public struct PlayerSettings
{
    [Header("Health")]
    public int maxHealth;

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float acceleration;

    [Header("Physics Material")]
    public PhysicMaterial staticPhysicsMaterial;
    public PhysicMaterial dynamicPhysicsMaterial;
}

[System.Serializable]
public struct ControlSettings
{
    [Range(0f,1f)]
    public float xSensitivity;
    [Range(0f, 1f)]
    public float ySensitivity;
    public bool yInverted;
    public float pitchClamp;
}
