using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [System.Serializable]
    public struct DoorJoint
    {
        public HingeJoint joint;
        public float previousAngle;
        public float delta;
    }
    [Header("Joints")]
    public DoorJoint[] joints;

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

        for(int i = 0; i < joints.Length; i++)
        {
            joints[i].previousAngle = joints[i].joint.angle;
        }

        obstacle.carving = locked;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].delta = Mathf.Abs(
                joints[i].joint.angle - joints[i].previousAngle) / Time.fixedDeltaTime;
            joints[i].previousAngle = joints[i].joint.angle;
        }

        for (int i = 0; i < joints.Length; i++)
        {
            if (justUnlocked && joints[i].delta > 6f)
            {
                onDoorUnlocked.Invoke();
                justUnlocked = false;
                break;
            }
            else if (!locked && joints[i].delta > 35f)
            {
                onDoorSwing.Invoke();
                break;
            }
            else if (locked && joints[i].delta > 0.8f)
            {
                onDoorLocked.Invoke();
                break;
            }
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
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].joint.limits = limits;
        }
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
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].joint.limits = limits;
        }
    }
}
