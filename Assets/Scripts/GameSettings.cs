using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName="SO_GameSettings",
    menuName="Scriptable Objects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public PlayerSettings playerSettings;
    public ControlSettings controlSettings;
    public PropData[] propDataset;
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

[System.Serializable]
public struct PropData
{
    [Header("ID")]
    public int id;
    public string name;

    [Header("Stats")]
    public int hitCount;
    public int damage;
}
