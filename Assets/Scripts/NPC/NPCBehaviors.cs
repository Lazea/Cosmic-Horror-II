using UnityEngine;
using Panda;
using UnityEngine.AI;
using UnityEngine.Events;

public class NPCBehaviors : MonoBehaviour
{
    public NPCBlackboard blackboard;

    [Header("Waypoints")]
    public int waypointManagerID = -1;
    public Waypoint currentWaypoint;

    [Header("Movement")]
    public float minRunChance = 0.2f;
    public float maxRunChance = 0.8f;
    float runChance;
    bool runningDiceRollInvoked;
    public float minStoppingDistance = 1f;
    public float maxStoppingDistance = 1.2f;
    public float minAngularSpeed = 280f;
    public float maxAngularSpeed = 320f;
    public float minPlayerOffsetRadius = 0.25f;
    public float maxPlayerOffsetRadius = 1.25f;
    public float minDestinationOffsetRadius = 1f;
    public float maxDestinationOffsetRadius = 2.5f;
    [SerializeField]
    Vector3 destinationOffset;

    [Header("Movement Values")]
    [SerializeField]
    Vector3 rootPosition;
    public float agentPositionSmoothing;

    [Header("Percetion")]
    public Transform head;
    public float sightRange =  15f;
    public float sightFOV = 200f;
    public float detectionRange = 4f;
    public LayerMask coverMask;

    [Header("Attack")]
    public float attackRange = 3.5f;
    public float aimSmooth = 0.4f;
    public int lightAttackDamage = 2;
    public int heavyAttackDamage = 6;
    public GameObject lightAttackHurtbox;
    public GameObject heavyAttackHurtbox;
    bool attackPerformed;

    [Header("Projectile Attack")]
    public int projectileAttackDamage = 10;
    public float projectileAttackRange = 12f;
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;

    [Header("Dead Ragdoll Lifetime")]
    public float deadNPCRagdollLifetime = 180f;

    [Header("Events")]
    public UnityEvent onSpawnState;
    public UnityEvent onPassiveState;
    public UnityEvent onChaseState;
    public UnityEvent onAttackState;
    public UnityEvent onLightAttack;
    public UnityEvent onHeavyAttack;
    public UnityEvent onProjectileAttack;
    public UnityEvent onHurt;
    public UnityEvent onDead;

    // Components
    // Animator
    Animator anim;
    bool hasWalkParam;
    bool hasRunParam;
    bool hasAttackParam;
    bool hasLightAttackLeftParam;
    bool hasLightAttackRightParam;
    bool hasProjectileAttackParam;
    bool hasHeavyAttackParam;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = true;

        hasWalkParam = AnimHasParameter(anim, "Walk");
        hasRunParam = AnimHasParameter(anim, "Run");
        hasAttackParam = AnimHasParameter(anim, "Attack");
        hasLightAttackLeftParam = AnimHasParameter(anim, "LightAttackLeft");
        hasLightAttackRightParam = AnimHasParameter(anim, "LightAttackRight");
        hasProjectileAttackParam = AnimHasParameter(anim, "ProjectileAttack");
        hasHeavyAttackParam = AnimHasParameter(anim, "HeavyAttack");

        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 50);
        agent.stoppingDistance = Random.Range(1f, 1.2f);
        agent.angularSpeed = Random.Range(500f, 700f);
        agent.updatePosition = false;

        blackboard.player = NPCsManager.Instance.Player;
        blackboard.playerTransform = blackboard.player.transform;

        runChance = Random.Range(minRunChance, maxRunChance);

        SetupHurtboxes();

        blackboard.state = NPCBlackboard.NPCState.Passive;

        onSpawnState.Invoke();
    }

    void SetupHurtboxes()
    {
        if (lightAttackHurtbox != null)
        {
            var lightLeftHurtbox = lightAttackHurtbox.GetComponent<NPCHurtbox>();
            lightLeftHurtbox.damage = lightAttackDamage;
            lightLeftHurtbox.owner = gameObject;
            lightAttackHurtbox.SetActive(false);
        }

        if (heavyAttackHurtbox != null)
        {
            var heavyHurtbox = heavyAttackHurtbox.GetComponent<NPCHurtbox>();
            heavyHurtbox.damage = heavyAttackDamage;
            heavyHurtbox.owner = gameObject;
            heavyAttackHurtbox.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead())
            return;

        CheckForPlayer();
        if(PlayerIsSpotted())
        {
            if (PlayerInAttackRange())
            {
                SetAttack();
            }
            else if(projectilePrefab != null && PlayerInProjectileAttackRange())
            {
                SetRangeAttack();
            }
            else
            {
                SetChase();
            }
        }
        else
        {
            SetPassive();
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        agent.isStopped = blackboard.isStopped;

        // Set animator movement params
        if(agent.isStopped)
        {
            if(hasWalkParam)
                anim.SetBool("Walk", false);
            if(hasRunParam)
                anim.SetBool("Run", false);
            else
                anim.SetBool("Walk", false);
        }
        else if (blackboard.isRunning)
        {
            if (hasWalkParam)
                anim.SetBool("Walk", false);
            if (hasRunParam)
                anim.SetBool("Run", true);
            else
                anim.SetBool("Walk", true);
        }
        else
        {
            if (hasWalkParam)
                anim.SetBool("Walk", true);
            if (hasRunParam)
                anim.SetBool("Run", false);
        }

        // Set running state based on player distance or random chance
        if(IsInChase())
        {
            if(agent.remainingDistance <= sightRange * 0.5f)
            {
                if(!runningDiceRollInvoked)
                {
                    runningDiceRollInvoked = true;
                    InvokeRepeating(nameof(RunningDiceRoll), 4f, 4f);
                }
            }
            else
            {
                if(runningDiceRollInvoked)
                {
                    runningDiceRollInvoked = false;
                    CancelInvoke(nameof(RunningDiceRoll));
                }
                blackboard.isRunning = true;
            }
        }

        // Aim at player if attacking
        if(IsInAttack())
        {
            agent.updateRotation = false;
            Vector3 playerPos = blackboard.playerTransform.position;
            playerPos.y = transform.position.y;
            Vector3 dir = playerPos - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                lookRot,
                aimSmooth * 20f * Time.deltaTime);
        }
        else
        {
            agent.updateRotation = true;
        }
    }

    void RunningDiceRoll()
    {
        blackboard.isRunning = (Random.Range(0f, 1f) >= runChance);
    }

    private void OnAnimatorMove()
    {
        rootPosition = anim.rootPosition;
        rootPosition.y = agent.nextPosition.y;

        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    private void LateUpdate()
    {
        float agentDist = Vector3.Distance(transform.position, agent.nextPosition);
        if (agentDist > 0.15f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                agent.nextPosition,
                agentPositionSmoothing);
        }
    }

    #region [Movement]
    [Task]
    public void StopMoving()
    {
        blackboard.isStopped = true;
        Task.current.Succeed();
    }

    [Task]
    public void ResumeMoving()
    {
        blackboard.isStopped = false;
        Task.current.Succeed();
    }

    [Task]
    public void SetRunning(bool running)
    {
        blackboard.isRunning = running;
        Task.current.Succeed();
    }

    [Task]
    public void SetPlayerDestinationOffset()
    {
        destinationOffset = GetDestinationOffset(
            minPlayerOffsetRadius, maxPlayerOffsetRadius);
        Task.current.Succeed();
    }

    [Task]
    public void SetDestinationOffset()
    {
        destinationOffset = GetDestinationOffset(
            minPlayerOffsetRadius, maxPlayerOffsetRadius);
        Task.current.Succeed();
    }

    public Vector3 GetDestinationOffset(float minOffset, float maxOffset)
    {
        float r = Random.Range(minOffset, maxOffset);
        Vector3 offset = Random.insideUnitCircle * r;
        return new Vector3(offset.x, 0f, offset.y);
    }

    [Task]
    public void GoToDestination()
    {
        if(!HasValidPath())
        {
            Task.current.Fail();
        }
        else if(agent.remainingDistance <= agent.stoppingDistance)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void ChasePlayer()
    {
        Vector3 playerPos = blackboard.playerTransform.position;
        agent.SetDestination(playerPos + destinationOffset);

        GoToDestination();
    }

    bool HasValidPath()
    {
        return agent.path.status == NavMeshPathStatus.PathComplete;
    }

    #region [Waypoints]
    [Task]
    public bool HasWaypointRoute()
    {
        return waypointManagerID >= 0;
    }

    [Task]
    public bool HasWaypoint()
    {
        if (currentWaypoint != null)
            if (currentWaypoint.transform != null)
                return true;
        return false;
    }

    [Task]
    public void SetRandomWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            Task.current.Fail();
            return;
        }

        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID).
            GetRandomWaypoint();
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public void SetClosestWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            Task.current.Fail();
            return;
        }

        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID).
            GetClosestWaypoint(transform.position);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public void SetNextWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            Task.current.Fail();
            return;
        }

        if(!HasWaypoint())
        {
            Task.current.Fail();
            return;
        }

        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID).
            GetNextWaypoint(currentWaypoint);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public void SetPreviousWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            Task.current.Fail();
            return;
        }

        if (!HasWaypoint())
        {
            Task.current.Fail();
            return;
        }

        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID).
            GetPreviousWaypoint(currentWaypoint);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
        Task.current.Succeed();
    }
    #endregion
    #endregion

    #region [Perception]
    public bool PlayerIsSpotted()
    {
        return blackboard.playerInSight;
    }

    public bool PlayerInAttackRange()
    {
        bool check = NPCsManager.IsPointInSight(
            head.position,
            head.forward,
            blackboard.playerTransform.position,
            360f,
            attackRange,
            coverMask);
        return check;
    }

    public bool PlayerInProjectileAttackRange()
    {
        bool check = NPCsManager.IsPointInSight(
            head.position,
            head.forward,
            blackboard.playerTransform.position,
            sightFOV,
            projectileAttackRange,
            coverMask);
        return check;
    }

    public bool PlayerInRange(float range)
    {
        return blackboard.playerDisplacement.magnitude <= range;
    }

    public bool PlayerIsDead()
    {
        return blackboard.IsPlayerDead;
    }

    public bool CheckForPlayer()
    {
        if (blackboard.IsPlayerDead)
        {
            blackboard.playerInSight = false;
            blackboard.playerDisplacement = Vector3.positiveInfinity;
            return false;
        }

        if (blackboard.playerInSight)
        {
            UpdatePlayerDisplacement();
            return true;
        }

        if (NPCsManager.IsPointInRange(
            head.position,
            blackboard.playerTransform.position,
            detectionRange))
        {
            UpdatePlayerDisplacement();
            blackboard.playerInSight = true;
            return true;
        }
        else if (NPCsManager.IsPointInSight(
            head.position,
            head.forward,
            blackboard.playerTransform.position,
            sightFOV,
            sightRange,
            coverMask))
        {
            UpdatePlayerDisplacement();
            blackboard.playerInSight = true;
            return true;
        }

        blackboard.playerInSight = false;
        return false;
    }

    void UpdatePlayerDisplacement()
    {
        Vector3 npcPos = transform.position + Vector3.up * agent.height * 0.5f;
        blackboard.playerDisplacement = blackboard.playerTransform.position - npcPos;
    }
    #endregion

    #region [Attack]
    [Task]
    public void PerformAttack()
    {
        if (blackboard.isAttacking)
        {
            return;
        }
        else if (attackPerformed)
        {
            attackPerformed = false;
            Task.current.Succeed();
            return;
        }

        if (hasLightAttackRightParam)
            anim.ResetTrigger("LightAttackRight");
        if (hasHeavyAttackParam)
            anim.ResetTrigger("HeavyAttack");
        if (hasProjectileAttackParam)
            anim.ResetTrigger("ProjectileAttack");
        if (hasLightAttackLeftParam)
            anim.ResetTrigger("LightAttackLeft");
        if (hasAttackParam)
            anim.SetTrigger("Attack");
        attackPerformed = true;
        blackboard.isAttacking = true;
    }

    [Task]
    public void PerformLeftLightAttack()
    {
        if (blackboard.isAttacking)
        {
            return;
        }
        else if (attackPerformed)
        {
            attackPerformed = false;
            Task.current.Succeed();
            return;
        }

        if (hasLightAttackRightParam)
            anim.ResetTrigger("LightAttackRight");
        if (hasHeavyAttackParam)
            anim.ResetTrigger("HeavyAttack");
        if (hasProjectileAttackParam)
            anim.ResetTrigger("ProjectileAttack");
        if (hasAttackParam)
            anim.ResetTrigger("Attack");
        if (hasLightAttackLeftParam)
            anim.SetTrigger("LightAttackLeft");
        attackPerformed = true;
        blackboard.isAttacking = true;
    }

    [Task]
    public void PerformRightLightAttack()
    {
        if (blackboard.isAttacking)
        {
            return;
        }
        else if (attackPerformed)
        {
            attackPerformed = false;
            Task.current.Succeed();
            return;
        }

        if (hasLightAttackLeftParam)
            anim.ResetTrigger("LightAttackLeft");
        if (hasHeavyAttackParam)
            anim.ResetTrigger("HeavyAttack");
        if (hasProjectileAttackParam)
            anim.ResetTrigger("ProjectileAttack");
        if (hasAttackParam)
            anim.ResetTrigger("Attack");
        if (hasLightAttackRightParam)
            anim.SetTrigger("LightAttackRight");
        attackPerformed = true;
        blackboard.isAttacking = true;
    }

    [Task]
    public void PerformHeavyAttack()
    {
        if (blackboard.isAttacking)
        {
            return;
        }
        else if (attackPerformed)
        {
            attackPerformed = false;
            Task.current.Succeed();
            return;
        }

        if (hasLightAttackLeftParam)
            anim.ResetTrigger("LightAttackLeft");
        if (hasLightAttackRightParam)
            anim.ResetTrigger("LightAttackRight");
        if (hasProjectileAttackParam)
            anim.ResetTrigger("ProjectileAttack");
        if (hasAttackParam)
            anim.ResetTrigger("Attack");
        if (hasHeavyAttackParam)
            anim.SetTrigger("HeavyAttack");
        attackPerformed = true;
        blackboard.isAttacking = true;
    }

    [Task]
    public void PerformProjectileAttack()
    {
        if (blackboard.isAttacking)
        {
            return;
        }
        else if (attackPerformed)
        {
            attackPerformed = false;
            Task.current.Succeed();
            return;
        }

        if (hasLightAttackLeftParam)
            anim.ResetTrigger("LightAttackLeft");
        if (hasLightAttackRightParam)
            anim.ResetTrigger("LightAttackRight");
        if (hasHeavyAttackParam)
            anim.ResetTrigger("HeavyAttack");
        if (hasAttackParam)
            anim.ResetTrigger("Attack");
        if (hasProjectileAttackParam)
            anim.SetTrigger("ProjectileAttack");
        attackPerformed = true;
        blackboard.isAttacking = true;
    }

    public void ReleaseAttack()
    {
        blackboard.isAttacking = false;
    }

    #region [Hurtboxes]
    public void EnableLightAttackLeftHurtbox(int enable)
    {
        onLightAttack.Invoke();
        lightAttackHurtbox.SetActive(enable > 0);
    }

    public void EnableLightAttackRightHurtbox(int enable)
    {
        onLightAttack.Invoke();
        lightAttackHurtbox.SetActive(enable > 0);
    }

    public void EnableHeavyAttackHurtbox(int enable)
    {
        onHeavyAttack.Invoke();
        heavyAttackHurtbox.SetActive(enable > 0);
    }

    public void FireProjectile()
    {
        onProjectileAttack.Invoke();
        var projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation,
            null);
        projectile.GetComponentInChildren<NPCHurtbox>().damage = projectileAttackDamage;
    }
    #endregion
    #endregion

    #region [States]
    #region [Setters]
    public void SetPassive()
    {
        attackPerformed = false;
        ReleaseAttack();
        blackboard.state = NPCBlackboard.NPCState.Passive;
    }

    public void SetChase()
    {
        attackPerformed = false;
        ReleaseAttack();
        blackboard.state = NPCBlackboard.NPCState.Chase;
    }

    public void SetRangeAttack()
    {
        blackboard.state = NPCBlackboard.NPCState.RangeAttack;
    }

    public void SetAttack()
    {
        blackboard.state = NPCBlackboard.NPCState.Attack;
    }

    public void SetDead()
    {
        attackPerformed = false;
        ReleaseAttack();
        blackboard.state = NPCBlackboard.NPCState.Dead;
    }
    #endregion

    #region [Checks]
    [Task]
    public bool IsInPassive()
    {
        return blackboard.state == NPCBlackboard.NPCState.Passive;
    }

    [Task]
    public bool IsInChase()
    {
        return blackboard.state == NPCBlackboard.NPCState.Chase;
    }

    [Task]
    public bool IsInRangeAttack()
    {
        return blackboard.state == NPCBlackboard.NPCState.RangeAttack;
    }

    [Task]
    public bool IsInAttack()
    {
        return blackboard.state == NPCBlackboard.NPCState.Attack;
    }

    [Task]
    public bool IsInCombat()
    {
        return IsInChase() || IsInRangeAttack() || IsInAttack();
    }

    [Task]
    public bool IsDead()
    {
        return blackboard.state == NPCBlackboard.NPCState.Dead;
    }

    [Task]
    public bool IsAttacking()
    {
        return blackboard.isAttacking;
    }
    #endregion
    #endregion

    #region [Health]
    public void DealDamage(
        int damage,
        NPCHitbox.HitboxType hitboxType,
        Vector3 hitForce,
        Vector3 hitPoint = default,
        GameObject damageSource = null)
    {
        //if (hitboxType == NPCHitbox.HitboxType.Head)
        //    damage = (int)(damage * 2.5f);
        blackboard.health = Mathf.Max(blackboard.health - damage, 0);

        Vector3 hitDir = hitForce;
        hitDir.y = 0f;
        hitDir.Normalize();
        float dot = Vector3.Dot(hitDir, transform.right);

        if (blackboard.health <= 0)
        {
            KillNPC((dot < 0f));
        }
        else
        {
            PlayHurtReaction(hitboxType, (dot < 0f));
            onHurt.Invoke();
        }
    }

    void PlayHurtReaction(NPCHitbox.HitboxType hitboxType, bool leftHit)
    {
        DisableAttackState();

        // TODO: Utilize when having different body part reactions
        //switch (hitboxType)
        //{
        //    case NPCHitbox.HitboxType.Head:
        //        anim.SetTrigger("HurtHead");
        //        break;
        //    case NPCHitbox.HitboxType.Leg:
        //        anim.SetTrigger("HurtHead");
        //        //anim.SetTrigger("HurtLeg");   TODO: Uncommend this once animations exist
        //        break;
        //    default:
        //        anim.SetTrigger("HurtHead");
        //        //anim.SetTrigger("HurtBody");  TODO: Uncommend this once animations exist
        //        break;
        //}

        if (leftHit)
        {
            anim.SetTrigger("HurtLeft");
            anim.ResetTrigger("HurtRight");
        }
        else
        {
            anim.SetTrigger("HurtRight");
            anim.ResetTrigger("HurtLeft");
        }
    }

    [ContextMenu("Kill NPC")]
    public void KillNPC(bool leftHit)
    {
        SetDead();

        DisableAttackState();

        anim.SetBool("Dead", true);
        
        // Call correct death animation
        if(Random.Range(0f, 1f) >= 0.45f)
        {
            if(leftHit)
            {
                anim.ResetTrigger("DeathBack");
                anim.ResetTrigger("DeathRight");
                anim.SetTrigger("DeathLeft");
            }
            else
            {
                anim.ResetTrigger("DeathBack");
                anim.ResetTrigger("DeathLeft");
                anim.SetTrigger("DeathRight");
            }
        }
        else
        {
            anim.ResetTrigger("DeathRight");
            anim.ResetTrigger("DeathLeft");
            anim.SetTrigger("DeathBack");
        }

        onDead.Invoke();
        Destroy(gameObject, deadNPCRagdollLifetime);
    }

    void DisableAttackState()
    {
        // Reset triggers
        if (hasLightAttackLeftParam)
            anim.ResetTrigger("LightAttackLeft");
        if (hasLightAttackRightParam)
            anim.ResetTrigger("LightAttackRight");
        if (hasHeavyAttackParam)
            anim.ResetTrigger("HeavyAttack");
        if (hasAttackParam)
            anim.ResetTrigger("Attack");
        if (hasProjectileAttackParam)
            anim.ResetTrigger("ProjectileAttack");

        // Disable hurtboxes
        if (lightAttackHurtbox != null)
            lightAttackHurtbox.SetActive(false);
        if (heavyAttackHurtbox != null)
            heavyAttackHurtbox.SetActive(false);

        blackboard.isAttacking = false;
        attackPerformed = false;
    }

    [ContextMenu("Kill NPC")]
    public void TestKillNPC()
    {
        KillNPC(true);
    }
    #endregion

    public bool AnimHasParameter(Animator _anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in _anim.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (IsDead())
            return;

        if(!IsInCombat())
        {
            if (head != null)
            {
                Gizmos.color = (IsInCombat()) ? Color.red : Color.yellow;

                Vector3 pos = head.position + Vector3.up * 0.05f;
                Gizmos.DrawWireSphere(pos, sightRange);
                Vector3 forward = new Vector3(
                head.forward.x,
                0f,
                head.forward.z).normalized;
                Gizmos.DrawLine(
                    pos,
                    pos + forward * sightRange);
                Gizmos.DrawLine(
                    pos,
                    pos + Quaternion.Euler(Vector3.up * sightFOV * 0.5f) * forward * sightRange);
                Gizmos.DrawLine(
                    pos,
                    pos + Quaternion.Euler(-Vector3.up * sightFOV * 0.5f) * forward * sightRange);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if(projectilePrefab != null)
        {
            Gizmos.color = new Color(1f, 0.36f, 0f);
            Gizmos.DrawWireSphere(transform.position, projectileAttackRange);
        }
    }
}
