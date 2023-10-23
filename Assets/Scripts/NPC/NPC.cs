using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [Header("Stats")]
    public int health;

    [Header("Attack Stats")]
    public int lightAttackDamage;
    public int heavyAttackDamage;

    [Header("Perception")]
    public Transform head;
    public float detectionRange;
    public float sightFOV;
    public float sightRange;
    public LayerMask coverMask;

    [Header("Navigation")]
    public int waypointManagerID;
    public float turnSpeedSmoothing;
    public float maxNavDisp;
    public float minMoveSpeed;
    [SerializeField]
    float dot;
    [SerializeField]
    float moveSpeed;
    float finalSpeed;
    [SerializeField]
    float animSpeed;
    float targetAnimSpeed;
    [SerializeField]
    bool stopped;
    [SerializeField]
    bool playerSpotted;
    Vector3 worldDeltaPosition;
    AnimatorStateInfo animState
    {
        get
        {
            if (anim != null)
                return anim.GetCurrentAnimatorStateInfo(1);
            else
                return new AnimatorStateInfo();
        }
    }

    [Header("Waiting")]
    public float minReactionTime;
    public float maxReactionTime;
    public float minPositionIdleTime;
    public float maxPositionIdleTime;
    bool isWaiting;

    // Player Stats
    Player player { get { return NPCsManager.Instance.Player; } }
    float playerOffsetRadius;
    Vector3 playerOffset;

    [Header("Events")]
    public UnityEvent onEnterPassive;
    public UnityEvent onEnterCombat;
    public UnityEvent onLightAttack;
    public UnityEvent onComboAttack;
    public UnityEvent onHeavyttack;
    public UnityEvent onHurt;
    public UnityEvent onDead;

    // Components
    NavMeshAgent agent;
    Animator anim;
    NPCRagdollController ragdollController;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = UnityEngine.Random.Range(0, 50);
        agent.stoppingDistance = UnityEngine.Random.Range(1f, 1.5f);
        agent.updatePosition = false;
        agent.updateRotation = true;

        anim = GetComponent<Animator>();
        anim.applyRootMotion = true;
        anim.SetBool("AI_Start", false);
        anim.SetBool("AI_Passive", true);

        ragdollController = GetComponent<NPCRagdollController>();
        ragdollController.DisableRagdoll();

        playerOffsetRadius = UnityEngine.Random.Range(3f, 4f);
        SetPlayerPositionOffset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        if (animState.IsName("Start"))
        {
            StopMoving();
        }
        else if (animState.IsName("Passive"))
        {
            //ResumeMoving();
            CheckForPlayer();
        }
        else if (animState.IsName("Combat"))
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !stopped)
            {
                StopMoving();
                StartCoroutine(Wait(
                    () => {
                        SetPlayerPositionOffset();
                        SetPlayerDestination(player.transform);
                        ResumeMoving();
                    },
                    minPositionIdleTime,
                    maxPositionIdleTime));
            }
            else
            {
                SetPlayerDestination(player.transform);
                MoveToDestination();
            }

            if(player.IsDead)
            {
                anim.SetBool("AI_Passive", true);
            }
        }
        else
        {
            StopMoving();
        }
    }

    #region [State Switcher]
    public void SwitchToCombatState()
    {
        anim.SetBool("AI_Start", false);
        anim.SetBool("AI_Passive", false);
    }

    public void SwitchToPassiveState()
    {
        anim.SetBool("AI_Start", false);
        anim.SetBool("AI_Passive", true);
    }

    public void SwitchToDeadState()
    {
        anim.SetBool("AI_Start", false);
        anim.SetBool("AI_Passive", false);
        anim.SetBool("AI_Dead", true);
    }
    #endregion

    #region [Movement]
    public void MoveToDestination()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0f;

        dot = 0f;
        if (!stopped)
        {
            dot = Vector3.Dot(transform.forward, worldDeltaPosition.normalized);
            dot = Mathf.Abs(dot);
        }
        moveSpeed = Mathf.Lerp(
            moveSpeed,
            dot,
            turnSpeedSmoothing);

        animSpeed = anim.GetFloat("Speed");
        anim.SetFloat(
            "Speed",
            Mathf.Lerp(
                animSpeed,
                moveSpeed,
                agent.acceleration));
    }

    public void SetPlayerDestination(Transform playerTransform)
    {
        Vector3 destination = NPCsManager.GetNavMeshPosition(
            playerTransform.position + playerOffset,
            playerOffsetRadius);
        agent.SetDestination(destination);
    }

    void SetPlayerPositionOffset()
    {
        Vector3 offset = UnityEngine.Random.insideUnitCircle;
        playerOffset = new Vector3(offset.x, 0f, offset.y) * playerOffsetRadius;
    }

    public void StopMoving()
    {
        Debug.Log("Stop Moving");
        stopped = true;
    }

    public void ResumeMoving()
    {
        Debug.Log("Resume Moving");
        stopped = false;
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = anim.rootPosition;
        rootPosition.y = agent.nextPosition.y;

        if (worldDeltaPosition.magnitude > maxNavDisp)
        {
            rootPosition = rootPosition + worldDeltaPosition;
        }
        transform.position = rootPosition;

        agent.nextPosition = rootPosition;
    }
    #endregion

    #region [Perception]
    public void CheckForPlayer()
    {
        if (playerSpotted)
            return;

        if(animState.IsName("Passive"))
        {
            if (NPCsManager.IsPointInRange(
                head.position,
                player.transform.position,
                detectionRange))
            {
                playerSpotted = true;

                StopMoving();
                StartCoroutine(Wait(() => {
                        SwitchToCombatState();
                        ResumeMoving();
                    },
                    minReactionTime,
                    maxReactionTime));
            }
            else if (NPCsManager.IsPointInSight(
                head.position,
                head.forward,
                player.transform.position,
                sightFOV,
                sightRange,
                coverMask))
            {
                playerSpotted = true;

                StopMoving();
                StartCoroutine(Wait(() => {
                        SwitchToCombatState();
                        ResumeMoving();
                    },
                    minReactionTime,
                    maxReactionTime));
            }
        }
    }
    #endregion

    #region [Health]
    public void DealDamage(
        int damage,
        NPCHitbox.HitboxType hitboxType,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = null)
    {
        if (hitboxType == NPCHitbox.HitboxType.Head)
            damage = (int)(damage * 2.5f);
        health -= damage;

        // TODO: Update to change direction and hit intensity
        PlayHurtReaction(
            hitboxType,
            true,
            true);

        if (health <= 0)
        {
            KillNPC();
        }
        else
        {
            onHurt.Invoke();
        }
    }

    void PlayHurtReaction(
        NPCHitbox.HitboxType hitboxType,
        bool leftHit,
        bool lightHit)
    {
        switch(hitboxType)
        {
            case NPCHitbox.HitboxType.Head:
                anim.SetTrigger("HurtHead");
                break;
            case NPCHitbox.HitboxType.Leg:
                anim.SetTrigger("HurtHead");
                //anim.SetTrigger("HurtLeg");   TODO: Uncommend this once animations exist
                break;
            default:
                anim.SetTrigger("HurtHead");
                //anim.SetTrigger("HurtBody");  TODO: Uncommend this once animations exist
                break;
        }
        anim.SetBool("HurtLeft", leftHit);
        anim.SetBool("HurtLight", lightHit);
    }

    #region [Hurt Tests]
    [ContextMenu("Light Hit Head Left")]
    public void TestLightHitHeadLeft()
    {
        PlayHurtReaction(NPCHitbox.HitboxType.Head, true, true);
    }

    [ContextMenu("Light Hit Head Right")]
    public void TestLightHitHeadRight()
    {
        PlayHurtReaction(NPCHitbox.HitboxType.Head, false, true);
    }
    #endregion

    [ContextMenu("Kill NPC")]
    public void KillNPC()
    {
        // TODO Replace with death state and ragdoll

        SwitchToDeadState();
        anim.SetBool("DeadBack", true);

        onDead.Invoke();
    }
    #endregion

    IEnumerator Wait(System.Action callback, float minTime, float maxTime)
    {
        float waitTime = UnityEngine.Random.Range(minTime, maxTime);
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;

        callback();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            transform.position + Vector3.up * 0.1f,
            transform.position + worldDeltaPosition.normalized + Vector3.up * 0.1f);
        Gizmos.color = Color.blue;
        Debug.DrawLine(
            transform.position + Vector3.up * 0.15f,
            transform.position + worldDeltaPosition + Vector3.up * 0.15f);
        Gizmos.color = Color.green;
        Debug.DrawLine(
            transform.position + Vector3.up * 0.2f,
            transform.position + transform.forward + Vector3.up * 0.2f);

        if(Application.isPlaying)
        {
            if (animState.IsName("Combat") || animState.IsName("Dead"))
                return;

            Gizmos.color = (animState.IsName("Combat")) ? Color.red : Color.yellow;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Vector3 pos = transform.position + Vector3.up * 0.05f;
        Gizmos.DrawWireSphere(pos, detectionRange);
        Gizmos.DrawWireSphere(pos, sightRange);
        Gizmos.DrawLine(
            pos,
            pos + transform.forward * sightRange);
        Gizmos.DrawLine(
            pos,
            pos + Quaternion.Euler(Vector3.up * sightFOV * 0.5f) * transform.forward * sightRange);
        Gizmos.DrawLine(
            pos,
            pos + Quaternion.Euler(-Vector3.up * sightFOV * 0.5f) * transform.forward * sightRange);
    }
}
