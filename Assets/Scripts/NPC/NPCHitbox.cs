using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHitbox : MonoBehaviour, IDamageable
{
    public enum HitboxType
    {
        Head,
        Body,
        Arm,
        Leg
    }
    public HitboxType type;

    public NPCBehavior npc;

    Rigidbody rb;

    private void Awake()
    {
        if(npc == null)
            npc = GetComponentInParent<NPCBehavior>();

        rb = GetComponent<Rigidbody>();
    }

    public void DealDamage(
        int damage,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = null)
    {
        if(!rb.isKinematic)
        {
            rb.AddForceAtPosition(
                hitForce,
                hitPoint,
                ForceMode.Impulse);
        }

        if(npc.enabled)
        {
            npc.DealDamage(
                damage,
                type,
                hitForce,
                hitPoint,
                damageSource);
        }
    }

    public void DestroyObject()
    {
        throw new System.NotImplementedException();
    }
}
