using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Transform checkpointTriggersParent;
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
        checkpointTriggersParent = GameObject.Find(
            "====Scene====/" +
            "====Interactables====/" +
            "====CheckpointTriggers====").transform;
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

            // Reset Checkpoint SO
            checkpoint.collectedKeys = null;
            checkpoint.disabledCheckpointTriggers.Clear();

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

    #region [Checkpoint Capture]
    [ContextMenu("Capture Checkpoint")]
    public void CaptureCheckpoint(GameObject sender)
    {
        if (checkpoint.disabledCheckpointTriggers.Contains(sender.name))
            return;

        CheckpointPlayerState(checkpoint);
        CheckpointKeys(checkpoint);

        checkpoint.disabledCheckpointTriggers.Add(sender.name);
        Destroy(sender);
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
        SetupCheckpointTriggers(checkpoint);
        SetupCollectedKeys(checkpoint);
        SetupDoorLocks();
    }

    public void SetupFromGameStartCheckpoint()
    {
        SetupPlayerState(gameStartCheckpoint);
        SetupCheckpointTriggers(gameStartCheckpoint);
        SetupCollectedKeys(gameStartCheckpoint);
        SetupDoorLocks();
    }

    public void SetupCheckpointTriggers(Checkpoint _checkpoint)
    {
        for (int i = checkpointTriggersParent.childCount - 1; i >= 0; i--)
        {
            foreach (var checkpointName in _checkpoint.disabledCheckpointTriggers)
            {
                var ceckpointT = checkpointTriggersParent.GetChild(i);
                if (ceckpointT.name == checkpointName)
                {
                    Destroy(ceckpointT.gameObject);
                    break;
                }
            }
        }
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

        for(int i = keysParent.childCount - 1; i >= 0; i--)
        {
            foreach (int keyID in _checkpoint.collectedKeys)
            {
                var keyT = keysParent.GetChild(i);
                if (keyT.GetComponent<KeyPickup>().keyID == keyID)
                {
                    Destroy(keyT.gameObject);
                    break;
                }
            }
        }
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
