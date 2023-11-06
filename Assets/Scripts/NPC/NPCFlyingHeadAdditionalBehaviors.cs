using UnityEngine;

public class NPCFlyingHeadAdditionalBehaviors : MonoBehaviour
{
    public GameObject npcHeadPrefab;
    public ParticleSystem splatterEffect;

    // Components
    Collider coll;
    Animator anim;
    NPCBehaviors behaviors;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();

        anim = GetComponent<Animator>();

        behaviors = GetComponent<NPCBehaviors>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySplatterEffect()
    {
        splatterEffect.Play();
    }

    public void SpawnCompleted()
    {
        coll.enabled = true;
        anim.SetTrigger("SpawnComplete");
    }

    public void SpawnHead()
    {
        Instantiate(
            npcHeadPrefab,
            transform.position,
            transform.rotation);

        // Set NPCBehaviors dead state
        behaviors.SetDead();

        // Reset triggers
        anim.ResetTrigger("LightAttackLeft");
        anim.ResetTrigger("LightAttackRight");
        anim.ResetTrigger("HeavyAttack");
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("ProjectileAttack");

        // Disable hurtboxes
        if (behaviors.lightAttackHurtbox != null)
            behaviors.lightAttackHurtbox.SetActive(false);
        if (behaviors.heavyAttackHurtbox != null)
            behaviors.heavyAttackHurtbox.SetActive(false);

        behaviors.onDead.Invoke();
        Destroy(gameObject, behaviors.deadNPCRagdollLifetime);
    }
}
