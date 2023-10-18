using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void DealDamage(
        int damage,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = default);

    public void DestroyObject();
}
