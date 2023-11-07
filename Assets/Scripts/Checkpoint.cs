using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SO_Checkpoint",
    menuName = "Scriptable Objects/Checkpoint")]
public class Checkpoint : ScriptableObject
{
    [Header("Player State")]
    public int playerHealth;
    public Vector3 playerSpawnPosition;
    public Quaternion playerSpawnOrientation;

    [Header("World State")]
    public int[] collectedKeys;
    public GameObject[] activeNPCWaves;
}
