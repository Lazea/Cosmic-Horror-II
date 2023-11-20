using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour, IDamageable
{
    public int durability;
    public PropType propType;
    public PropMaterial propMaterial;
    public PropWeight propWeight;

    [Header("Effects")]
    public float damageEffectSize = 1f;
    public GameObject damageEffect;
    public float destroyEffectSize = 1f;
    public Transform destroyEffectPoint;
    public GameObject destroyEffect;

    [Header("Events")]
    public UnityEvent onPropImpact;
    public UnityEvent onPropDamaged;
    public UnityEvent onPropDestroyed;

    public float componentsLiftime = 4f;
    public Behaviour[] componentsToDetachOnDestroy;

    // Components
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DealDamage(
        int damage,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = default)
    {
        durability -= damage;

        if (hitPoint == default)
        {
            rb.AddForce(hitForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForceAtPosition(
                hitForce,
                hitPoint,
                ForceMode.Impulse);
        }

        onPropDamaged.Invoke();

        if (durability <= 0)
        {
            foreach (var b in componentsToDetachOnDestroy)
            {
                b.transform.parent = null;
                Destroy(b.gameObject, componentsLiftime);
            }

            DestroyObject();
        }
        //else if (damageEffect != null)
        //{
        //    Vector3 point = (hitPoint == default) ? transform.position : hitPoint;
        //    GameObject.Instantiate(
        //        damageEffect,
        //        point,
        //        transform.rotation);
        //}
    }

    [ContextMenu("Deal 1 Damage")]
    public void TestDealDamage()
    {
        DealDamage(1, Vector3.zero);
    }

    [ContextMenu("Deal 1 Damage with Force")]
    public void TestDealDamageWithForce()
    {
        DealDamage(1, Vector3.up * 8f);
    }

    [ContextMenu("Destroy Prop")]
    public void DestroyObject()
    {
        onPropDestroyed.Invoke();

        if (destroyEffect != null)
        {
            if(destroyEffectPoint == null)
            {
                var effect = GameObject.Instantiate(
                    destroyEffect,
                    transform.position,
                    transform.rotation);
                effect.transform.localScale *= destroyEffectSize;
            }
            else
            {
                var effect = GameObject.Instantiate(
                    destroyEffect,
                    destroyEffectPoint.position,
                    destroyEffectPoint.rotation);
                effect.transform.localScale *= destroyEffectSize;
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
            return;

        if (collision.impulse.magnitude > 6f && damageEffect != null)
        {
            Vector3 contactPoint = Vector3.zero;
            foreach (var c in collision.contacts)
            {
                contactPoint += c.point;
            }
            contactPoint /= collision.contacts.Length;

            GameObject.Instantiate(
                damageEffect,
                contactPoint,
                Quaternion.identity);
        }

        onPropImpact.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (destroyEffectPoint == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(destroyEffectPoint.position, 0.2f);
    }
}
