using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOGameEventSystem;
using UnityEngine.Events;
using SOGameEventSystem.Events;
using System.Linq;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    int health;
    public int Health { get { return health; } }
    public bool IsHurt
    {
        get { return (health < GameManager.Instance.settings.playerSettings.maxHealth); }
    }
    bool isDead;
    public bool IsDead
    {
        get { return isDead; }
    }

    List<int> keyIDs;
    public List<int> KeyIDs
    {
        get { return keyIDs; }
    }

    [Header("Events")]
    public UnityEvent onPlayerDamage;
    public UnityEvent onPlayerHeal;
    public UnityEvent<int> onPlayerHealthChange = new UnityEvent<int>();
    public IntGameEvent PlayerHealthChangeEvent;
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
        Debug.LogFormat("Player Health Changed to {0}", health);
        PlayerHealthChangeEvent.Raise(health);
        onPlayerHeal.Invoke();

        Destroy(healthPickup.gameObject);
    }

    public void SetKeyIDs(int[] _keyIDs)
    {
        keyIDs = _keyIDs.ToList<int>();
    }

    public void KeyPickup(KeyPickup keyPickup)
    {
        keyIDs.Add(keyPickup.keyID);
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
        if (health <= 0)
        {
            onPlayerHealthChange.Invoke(health);
            Debug.LogFormat("Player Health Changed to {0}", health);
            PlayerHealthChangeEvent.Raise(health);
            DestroyObject();
        }
        else
        {
            onPlayerHealthChange.Invoke(health);
            Debug.LogFormat("Player Health Changed to {0}", health);
            PlayerHealthChangeEvent.Raise(health);
            onPlayerDamage.Invoke();
        }
    }

    [ContextMenu("Kill Player")]
    public void DestroyObject()
    {
        if (isDead)
            return;

        health = 0;
        isDead = true;
        onPlayerDeath.Raise();
    }

    public void SetHealth(int health)
    {
        this.health = health;
        if(health < 0)
        {
            health = 0;
        }
        else
        if(health > GameManager.Instance.settings.playerSettings.maxHealth)
        {
            health = GameManager.Instance.settings.playerSettings.maxHealth;
        }

        onPlayerHealthChange.Invoke(health);
        Debug.LogFormat("Player Health Changed to {0}", health);
        PlayerHealthChangeEvent.Raise(health);
    }
}
