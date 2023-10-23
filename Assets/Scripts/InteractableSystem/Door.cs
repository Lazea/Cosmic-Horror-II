using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField]
    bool locked;
    bool justUnlocked;
    public bool Locked { get { return locked; } }
    public int keyID;

    [Header("Joint")]
    public HingeJoint joint;
    float previousAngle = 0f;
    float delta;

    [Header("Events")]
    public UnityEvent onDoorSwing;
    public UnityEvent onDoorLocked;
    public UnityEvent onDoorUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        if (locked)
            LockDoor();
        else
            UnlockDoor();

        previousAngle = joint.angle;
    }

    private void FixedUpdate()
    {
        delta = Mathf.Abs(joint.angle - previousAngle) / Time.fixedDeltaTime;
        previousAngle = joint.angle;

        if (locked && delta > 0.85f)
        {
            onDoorLocked.Invoke();
        }
        else if(!locked && delta > 35f)
        {
            onDoorSwing.Invoke();
        }
        else if(justUnlocked && delta > 0.85f)
        {
            onDoorUnlocked.Invoke();
            justUnlocked = false;
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

        JointLimits limits = new JointLimits();
        limits.min = -90f;
        limits.max = 90f;
        joint.limits = limits;
    }
}
