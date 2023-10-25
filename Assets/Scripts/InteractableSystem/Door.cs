using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField]
    bool locked;
    [SerializeField]
    bool justUnlocked;
    public bool Locked { get { return locked; } }
    public int keyID;

    [Header("Joint")]
    public HingeJoint joint;
    float previousAngle = 0f;
    [SerializeField]
    float delta;

    [Header("Events")]
    public UnityEvent onDoorSwing;
    public UnityEvent onDoorLocked;
    public UnityEvent onDoorUnlocked;

    // Components
    NavMeshObstacle obstacle;

    // Start is called before the first frame update
    void Start()
    {
        obstacle = GetComponentInChildren<NavMeshObstacle>();

        if (locked)
        {
            LockDoor();
        }
        else
        {
            UnlockDoor();
            justUnlocked = false;
        }

        previousAngle = joint.angle;

        obstacle.carving = locked;
    }

    private void FixedUpdate()
    {
        delta = Mathf.Abs(joint.angle - previousAngle) / Time.fixedDeltaTime;
        previousAngle = joint.angle;

        if (justUnlocked && delta > 6f)
        {
            onDoorUnlocked.Invoke();
            justUnlocked = false;
        }
        else if (!locked && delta > 35f)
        {
            onDoorSwing.Invoke();
        }
        else if (locked && delta > 0.8f)
        {
            onDoorLocked.Invoke();
        }
    }

    public void LockDoor(int keyID = default)
    {
        if (keyID != default)
        {
            this.keyID = keyID;
        }

        LockDoor();
    }

    [ContextMenu("Lock Door")]
    void LockDoor()
    {
        locked = true;
        obstacle.carving = locked;

        JointLimits limits = new JointLimits();
        limits.min = -1.5f;
        limits.max = 1.5f;
        joint.limits = limits;
    }

    public void UnlockDoor(int keyID)
    {
        if (this.keyID != keyID)
            return;

        UnlockDoor();
    }

    [ContextMenu("Unlock Door")]
    void UnlockDoor()
    {
        justUnlocked = true;
        locked = false;
        obstacle.carving = locked;

        JointLimits limits = new JointLimits();
        limits.min = -90f;
        limits.max = 90f;
        joint.limits = limits;
    }
}
