using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCHurtbox : MonoBehaviour
{
    public GameObject owner;
    public int damage;

    public UnityEvent onHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.DealDamage(damage, Vector3.zero, default, owner);
        }
        onHit.Invoke();
        gameObject.SetActive(false);
    }
}
