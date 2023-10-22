using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName="SO_GameSettings",
    menuName="Scriptable Objects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public PlayerSettings playerSettings;
    public ControlSettings controlSettings;
    public AudioMixer audioMixer;
    public PropSettings propSettings;
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
public struct PropSettings
{
    [Header("Attack Settings")]
    public float AttackShortRange;
    public float AttackLongRange;
    public float attackLightForce;
    public float attackHeavyForce;
    public AnimationCurve propImpactDamageCurve;
    [System.Serializable]
    public struct PropMaterialImpactDamageMultipier
    {
        public PropMaterial propMaterial;
        [Range(1f, 2f)]
        public float multipier;
    }
    public PropMaterialImpactDamageMultipier[] propMaterialImpactDamageMultipiers;

    [Header("Props Dataset")]
    public PropData[] propsDataset;
}

[System.Serializable]
public struct PropData
{
    [Header("ID")]
    public int id;
    public string name;
    public PropMaterial propMaterial;

    [Header("Stats")]
    public int durability;
    public int damage;
}

public enum PropMaterial
{
    Wood,
    Metal,
    Glass,
    Other
}
