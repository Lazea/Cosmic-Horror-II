using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class CheckpointSystem : Singleton<CheckpointSystem>
{
    [Header("Checkpoints")]
    public Checkpoint gameStartCheckpoint;
    public Checkpoint checkpoint;

    [Header("Game Start Objects")]
    public GameObject[] gameStartObjects;

    [Header("Parent Objects")]
    public Transform keysParent;
    public Transform doorsParent;
    public Transform waypointsParent;

    Player player
    {
        get
        {
            if (Application.isPlaying)
            {
                return GameManager.Instance.player;
            }
            else
            {
                return GameObject.Find("Player").GetComponent<Player>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        keysParent = GameObject.Find(
            "====Scene====/" +
            "====Interactables====/" +
            "====Keys====").transform;
        doorsParent = GameObject.Find(
            "====Scene====/" +
            "====Interactables====/" +
            "====Doors====").transform;
        waypointsParent = GameObject.Find(
            "====Scene====/" +
            "====NPCs====/" +
            "====Waypoints====").transform;

        if(GameManager.Instance.settings.isGameStart)
        {
            GameManager.Instance.settings.isGameStart = false;
            foreach (var g in gameStartObjects)
            {
                g.SetActive(true);
            }

            SetupFromGameStartCheckpoint();
        }
        else
        {
            foreach (var g in gameStartObjects)
            {
                g.SetActive(false);
            }

            SetupFromCheckpoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region [Checkpoint Capture]
    [ContextMenu("Capture Checkpoint")]
    public void CaptureCheckpoint()
    {
        CheckpointPlayerState(checkpoint);
        CheckpointKeys(checkpoint);
    }

    [ContextMenu("Capture Game Start Checkpoint")]
    public void CaptureGameStartCheckpoint()
    {
        CheckpointPlayerState(gameStartCheckpoint);
        gameStartCheckpoint.playerHealth = GameManager.Instance.settings.playerSettings.maxHealth;
        CheckpointKeys(gameStartCheckpoint);
    }

    public void CheckpointPlayerState(Checkpoint _checkpoint)
    {
        _checkpoint.playerHealth = player.Health;
        _checkpoint.playerSpawnPosition = player.transform.position;
        _checkpoint.playerSpawnOrientation = player.transform.rotation;
    }

    public void CheckpointKeys(Checkpoint _checkpoint)
    {
        _checkpoint.collectedKeys = player.KeyIDs.ToArray();
    }
    #endregion

    #region [Setup From Checkpoint]
    public void SetupFromCheckpoint()
    {
        SetupPlayerState(checkpoint);
        SetupCollectedKeys(checkpoint);
        SetupDoorLocks(checkpoint);
    }

    public void SetupFromGameStartCheckpoint()
    {
        SetupPlayerState(gameStartCheckpoint);
        SetupCollectedKeys(gameStartCheckpoint);
        SetupDoorLocks();
    }

    public void SetupPlayerState(Checkpoint _checkpoint)
    {
        player.transform.position = _checkpoint.playerSpawnPosition;
        player.transform.rotation = _checkpoint.playerSpawnOrientation;
        player.SetHealth(_checkpoint.playerHealth);
    }

    public void SetupCollectedKeys(Checkpoint _checkpoint)
    {
        player.SetKeyIDs(_checkpoint.collectedKeys);
    }

    public void SetupDoorLocks()
    {
        var keyIDs = player.KeyIDs;
        foreach (Transform doorT in doorsParent)
        {
            var door = doorT.GetComponent<Door>();
            foreach (int id in keyIDs)
            {
                if(door.keyID == id)
                {
                    door.UnlockDoor(id);
                    break;
                }
            }
        }
    }
    #endregion
}
