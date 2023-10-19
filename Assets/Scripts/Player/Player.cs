using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOGameEventSystem;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable
{
    public int health;
    public bool IsHurt
    {
        get { return (health < GameManager.Instance.settings.playerSettings.maxHealth); }
    }

    [Header("Events")]
    public UnityEvent onPlayerDamage;
    public UnityEvent onPlayerHeal;
    public UnityEvent<int> onPlayerHealthChange = new UnityEvent<int>();
    public BaseGameEvent onPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        health = GameManager.Instance.settings.playerSettings.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePlayer()
    {
        GetComponent<PlayerCharacterController>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        GetComponent<FirstPersonCameraController>().enabled = true;
        GetComponent<InteractableCheck>().enabled = true;
        GetComponent<PlayerPropController>().enabled = true;
    }

    public void DisablePlayer()
    {
        GetComponent<PlayerCharacterController>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<FirstPersonCameraController>().enabled = false;
        GetComponent<InteractableCheck>().enabled = false;
        GetComponent<PlayerPropController>().enabled = false;
    }

    public void HealthPickup(HealthPickup healthPickup)
    {
        health = Mathf.Min(
            health + healthPickup.healAmount,
            GameManager.Instance.settings.playerSettings.maxHealth);

        onPlayerHealthChange.Invoke(health);
        onPlayerHeal.Invoke();

        Destroy(healthPickup.gameObject);
    }

    public void KeyPickup(KeyPickup keyPickup)
    {
        Destroy(keyPickup.gameObject);
        // TODO: Add key to inventory
    }

    public void DealDamage(
        int damage,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = default)
    {
        health -= damage;
        if(health <= 0)
        {
            onPlayerHealthChange.Invoke(health);
            DestroyObject();
        }
        else
        {
            onPlayerHealthChange.Invoke(health);
            onPlayerDamage.Invoke();
        }
    }

    public void DestroyObject()
    {
        onPlayerDeath.Raise();
        // TODO: Play killed animation and end the game
    }
}
