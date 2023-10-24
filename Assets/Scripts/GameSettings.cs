using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName = "SO_GameSettings",
    menuName = "Scriptable Objects/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Player")]
    public PlayerSettings playerSettings;

    [Header("Controls")]
    public ControlSettings controlSettings;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Props")]
    public PropSettings propSettings;
    public TextAsset csvAsset;
    [ContextMenu("Parse CSV")]
    public void ParseCSV()
    {
        propSettings.propsDataset = CSVSerializer.Deserialize<PropData>(csvAsset.text);
        for (int i = 0; i < propSettings.propsDataset.Length; i++)
        {
            propSettings.propsDataset[i].id = i;
        }

        // TODO: Map prefabs to database items
        //string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/Prefabs/Props"});
        //foreach (var guid in guids)
        //{
        //    var path = AssetDatabase.GUIDToAssetPath(guid);
        //    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        //}
    }
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
    public struct PropMaterialImpactDamageMultiplier
    {
        public PropMaterial propMaterial;
        [Range(1f, 2f)]
        public float multiplier;
    }
    public PropMaterialImpactDamageMultiplier[] propMaterialImpactDamageMultipliers;

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
