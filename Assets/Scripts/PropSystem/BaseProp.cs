using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseProp : MonoBehaviour, IProp, IDamageable
{
    [Header("ID")]
    public int id;
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // TODO: Move this later to the game settings
    [System.Serializable]
    public enum PropType
    {
        OneHanded,
        TwoHanded,
        Medium,
    }
    public PropType propType;

    // TODO: Move this later to the game settings
    [System.Serializable]
    public enum PropAttackRange
    {
        Short,
        Long,
    }
    public PropAttackRange propAttackRange;

    // TODO: Move this later to the game settings
    [System.Serializable]
    public enum PropAttackForce
    {
        Light,
        Heavy,
    }
    public PropAttackForce propAttackForce;

    public PropMaterial propMaterial;
    PropSettings propSettings
    {
        get { return GameManager.Instance.settings.propSettings; }
    }

    [Header("Stats")]
    public int durability;
    public int damage;

    [Header("Status")]
    public bool isHeld = false;

    [Header("Effects")]
    public GameObject damageEffect;
    public GameObject destroyEffect;

    [Header("Events")]
    public UnityEvent onPropImpact;
    public UnityEvent onPropDamaged;
    public UnityEvent onPropDestroyed;

    // Components
    [HideInInspector]
    Rigidbody rb;
    public Rigidbody RB { get { return rb; } }
    [HideInInspector]
    Collider[] colliders;
    public Collider[] Colliders { get { return colliders; } }

    private void Awake()
    {
        foreach (var pd in propSettings.propsDataset)
        {
            if(pd.id == id)
            {
                propMaterial = pd.propMaterial;
                durability = pd.durability;
                damage = pd.damage;
            }
        }

        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DealDamage(
        int damage,
        Vector3 hitForce,
        Vector3 hitPoint=default,
        GameObject damageSource = default)
    {
        durability -= damage;

        if(hitPoint == default)
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
        else if(damageEffect != null)
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

        float impactForce = collision.impulse.magnitude / rb.mass;
        var otherRB = collision.collider.attachedRigidbody;
        if (otherRB != null)
        {
            impactForce *= 0.5f * otherRB.mass;
        }

        float impactMultipier = 1f;
        foreach (var s in propSettings.propMaterialImpactDamageMultipliers)
            if (s.propMaterial == propMaterial)
                impactMultipier = s.multiplier;

        int impactDamage = (int)(propSettings.propImpactDamageCurve.Evaluate(
            impactForce) * impactMultipier);
        Debug.Log(string.Format("[{0}] Impact Force: {1}; Damage: {2}",
            name,
            impactForce,
            impactDamage));
        if (impactDamage > 0)
            DealDamage(impactDamage, Vector3.zero);
    }
}
