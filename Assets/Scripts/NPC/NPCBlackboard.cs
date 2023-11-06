using UnityEngine;

[System.Serializable]
public struct NPCBlackboard
{
    public int health;
    public enum NPCState
    {
        Passive,
        Chase,
        Attack,
        RangeAttack,
        Dead
    }
    public NPCState state;
    public bool isRunning;
    public bool isStopped;
    public bool isAttacking;

    public Player player;
    public Transform playerTransform;
    public bool playerInSight;
    public Vector3 playerDisplacement;
    public bool IsPlayerDead
    {
        get { return player.IsDead; }
    }
}
