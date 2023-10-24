using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour, IDamageable
{
    public int durability;

    [Header("Effects")]
    public GameObject damageEffect;
    public GameObject destroyEffect;

    [Header("Events")]
    public UnityEvent onPropImpact;
    public UnityEvent onPropDamaged;
    public UnityEvent onPropDestroyed;

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
            DestroyObject();
        }
        else if (damageEffect != null)
        {
            Vector3 point = (hitPoint == default) ? transform.position : hitPoint;
            GameObject.Instantiate(
                damageEffect,
                point,
                transform.rotation);
        }
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
            GameObject.Instantiate(
                destroyEffect,
                transform.position,
                transform.rotation);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
            return;

        onPropImpact.Invoke();
    }
}
