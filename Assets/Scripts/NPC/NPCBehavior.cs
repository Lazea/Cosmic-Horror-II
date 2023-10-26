using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NPCBehavior : MonoBehaviour
{
    [Header("NPC Health")]
    public int health;

    [Header("NPC State")]
    public NPCState npcState = NPCState.Start;
    public enum NPCState
    {
        Start,
        Passive,
        Chase,
        Attack,
        Dead,
        Wait
    }
    [SerializeField]

    [Header("Perception")]
    public Transform head;
    public float detectionRange;
    public float sightFOV;
    public float sightRange;
    public LayerMask coverMask;
    [SerializeField]
    bool playerSpotted;

    [Header("Attacking")]
    public int lightAttackDamage;
    public int heavyAttackDamage;
    public float attackRange;
    public float attackAngle;
    public GameObject lightAttackLeftHurtbox;
    public GameObject lightAttackRightHurtbox;
    public GameObject heavyAttackHurtbox;
    [SerializeField]
    bool isAttacking;

    [Header("Navigation")]
    public int waypointManagerID;
    public Waypoint currentWaypoint;
    public float turnSpeedSmoothing;
    public float maxNavDisp;
    public float minMoveSpeed;
    [SerializeField]
    float dot;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float animSpeed;
    [SerializeField]
    bool stopped;
    [SerializeField]
    bool running;
    Vector3 worldDeltaPosition;
    [SerializeField]
    Vector3 positionOffset;
    AnimatorStateInfo animState
    {
        get
        {
            if (anim != null)
                return anim.GetCurrentAnimatorStateInfo(0);
            else
                return new AnimatorStateInfo();
        }
    }
    bool playerIsDestination;

    // Player Stats
    Player player { get { return NPCsManager.Instance.Player; } }

    [Header("Events")]
    public UnityEvent onSpawnState;
    public UnityEvent onPassiveState;
    public UnityEvent onChaseState;
    public UnityEvent onAttackState;
    public UnityEvent onLightAttack;
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
        agent.avoidancePriority = Random.Range(0, 50);
        agent.stoppingDistance = Random.Range(1f, 1.5f);
        agent.updatePosition = false;
        agent.updateRotation = true;

        anim = GetComponent<Animator>();
        anim.applyRootMotion = true;

        ragdollController = GetComponent<NPCRagdollController>();
        ragdollController.DisableRagdoll();

        running = false;

        SetupHurtboxes();

        SetPositionOffset(1.5f, 2.5f);

        SetStartState();
        SetPassiveState();  // TODO: Remove this once animation event calls the function

        onSpawnState.Invoke();
    }

    void SetupHurtboxes()
    {
        var lightLeftHurtbox = lightAttackLeftHurtbox.GetComponent<NPCHurtbox>();
        lightLeftHurtbox.damage = lightAttackDamage;
        lightLeftHurtbox.owner = gameObject;

        var lightRightHurtbox = lightAttackRightHurtbox.GetComponent<NPCHurtbox>();
        lightRightHurtbox.damage = lightAttackDamage;
        lightRightHurtbox.owner = gameObject;

        var heavyHurtbox = heavyAttackHurtbox.GetComponent<NPCHurtbox>();
        heavyHurtbox.damage = heavyAttackDamage;
        heavyHurtbox.owner = gameObject;

        lightAttackLeftHurtbox.SetActive(false);
        lightAttackRightHurtbox.SetActive(false);
        heavyAttackHurtbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        HandleNPCMovement();
        UpdateNPCState();

        if(!animState.IsTag("Attack"))
        {
            isAttacking = false;
        }

        switch (npcState)
        {
            case NPCState.Passive:
                HandlePatrol();
                break;
            case NPCState.Chase:
                HandleChase();
                break;
            case NPCState.Attack:
                HandleAttacking();
                break;
        }
    }

    void UpdateNPCState()
    {
        if (IsStartState())
        {
            return;
        }

        if (IsPassiveState())
        {
            if (playerSpotted)
            {
                StopMoving();
                StopAllCoroutines();
                SetPositionOffset(1.5f, 2.5f);
                SetWaitState();
                float waitT = Random.Range(0.2f, 0.75f);
                StartCoroutine(ExecuteDelayed(
                    () =>
                    {
                        SetChaseState();
                        StartCoroutine("SetRandomRunning");
                    },
                    waitT));
            }
        }
        else if (IsChaseState())
        {
            if (playerSpotted)
            {
                if (PlayerIsInFront())
                {
                    StopMoving();
                    StopAllCoroutines();
                    SetAttackState();
                }
            }
            else
            {
                StopMoving();
                StopAllCoroutines();
                SetWaitState();
                float waitT = Random.Range(1f, 3f);
                StartCoroutine(ExecuteDelayed(SetPassiveState, waitT));
            }
        }
        else if (IsAttackState())
        {
            if (playerSpotted)
            {
                if (!PlayerIsInFront())
                {
                    StopAllCoroutines();
                    SetChaseState();
                    StartCoroutine("SetRandomRunning");
                }
            }
            else
            {
                StopMoving();
                StopAllCoroutines();
                SetWaitState();
                float waitT = Random.Range(1f, 3f);
                StartCoroutine(ExecuteDelayed(SetPassiveState, waitT));
            }
        }
    }

    void HandlePatrol()
    {
        if (!HasWaypointRoute())
        {
            StopMoving();
            return;
        }

        if(HasWaypoint() && !NPCAtDestination())
        {
            ResumeMoving();
            return;
        }

        if(!HasWaypoint())
        {
            StopMoving();
            StopAllCoroutines();
            SetWaitState();
            float waitT = Random.Range(1f, 1.5f);
            StartCoroutine(ExecuteDelayed(
                () => {
                    SetClosestWaypointDestination();
                    SetPassiveState();
                },
                waitT));
        }
        else
        {
            StopMoving();
            StopAllCoroutines();
            SetWaitState();
            float waitT = Random.Range(1f, 1.5f);
            StartCoroutine(ExecuteDelayed(
                () => {
                    SetNextWaypointDestination();
                    SetPassiveState();
                },
                waitT));
        }
    }

    void HandleChase()
    {
        ResumeMoving();
        SetPlayerDestination();
    }

    IEnumerator SetRandomRunning()
    {
        while (true)
        {
            if (IsChaseState())
            {
                float distance = Vector3.Distance(
                    player.transform.position,
                    transform.position);
                float i = Random.Range(0f, 1f);
                if (distance >= sightRange * 0.5f)
                {
                    SetRunning((i >= 0.1f));
                }
                else
                {
                    SetRunning((i >= 0.8f));
                }
            }

            yield return new WaitForSeconds(3.75f);
        }
    }

    void HandleAttacking()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.y = transform.position.y;
        Vector3 dir = playerPos - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRot,
            0.45f * 20f * Time.deltaTime);

        if (isAttacking)
            return;

        int i = Random.Range(0, 4);
        float delay = Random.Range(0.1f, 0.65f);
        switch(i)
        {
            case 0:
                StartCoroutine(ExecuteDelayed(PerformLightAttackLeft, delay));
                break;
            case 1:
                StartCoroutine(ExecuteDelayed(PerformLightAttackRight, delay));
                break;
            case 2:
                StartCoroutine(ExecuteDelayed(PerformAttackCombo, delay));
                break;
            case 3:
                StartCoroutine(ExecuteDelayed(PerformHeavyAttack, delay));
                break;
            default:
                StartCoroutine(ExecuteDelayed(PerformLightAttackLeft, delay));
                break;
        }
    }

    void HandleNPCMovement()
    {
        if (playerIsDestination)
        {
            Vector3 destination = NPCsManager.GetNavMeshPosition(
                player.transform.position + positionOffset,
                positionOffset.magnitude);

            agent.SetDestination(destination);
        }

        worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0f;

        dot = 0f;
        if (!stopped)
        {
            dot = Vector3.Dot(transform.forward, worldDeltaPosition.normalized);
            dot = Mathf.Abs(dot);
        }

        if (!running)
        {
            dot *= 0.5f;
        }

        moveSpeed = Mathf.Lerp(
            moveSpeed,
            dot,
            turnSpeedSmoothing * 200f * Time.deltaTime);

        animSpeed = anim.GetFloat("Speed");
        anim.SetFloat(
            "Speed",
            Mathf.Lerp(
                animSpeed,
                moveSpeed,
                agent.acceleration));
    }

    IEnumerator ExecuteDelayed(System.Action callback, float time)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    #region [Perception]
    public bool CheckForPlayer()
    {
        if (player.IsDead)
        {
            playerSpotted = false;
            return false;
        }

        if (playerSpotted)
        {
            return true;
        }

        if (NPCsManager.IsPointInRange(
            head.position,
            player.transform.position,
            detectionRange))
        {
            playerSpotted = true;
            return true;
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
            return true;
        }

        playerSpotted = false;
        return false;
    }

    public bool PlayerIsInFront()
    {
        Vector3 npcPos = transform.position + Vector3.up * agent.height * 0.5f;
        Vector3 disp = player.transform.position - npcPos;
        float dist = disp.magnitude;
        if (dist <= attackRange)
        {
            Vector3 dir = disp.normalized;
            float a = Vector3.Angle(transform.forward, dir);
            if(a <= attackAngle * 0.5f)
            {
                Ray ray = new Ray(npcPos, dir);
                return !Physics.Raycast(
                    ray,
                    dist,
                    coverMask);
            }
        }

        return false;
    }
    #endregion

    #region [Movement]
    #region [Waypoints]
    public bool HasWaypointRoute()
    {
        return waypointManagerID >= 0;
    }

    public bool HasWaypoint()
    {
        if (currentWaypoint != null)
            if (currentWaypoint.transform != null)
                return true;
        return false;
    }

    public void SetRandomWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            return;
        }

        playerIsDestination = false;
        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID).
            GetRandomWaypoint();
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
    }

    public void SetClosestWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            return;
        }

        Debug.Log("Set Closest Waypoint Destination");
        playerIsDestination = false;
        currentWaypoint = NPCsManager.Instance.GetWaypointManager(waypointManagerID)
            .GetClosestWaypoint(transform.position);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
    }

    public void SetNextWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            return;
        }

        Debug.Log("Set Next Waypoint Destination");
        playerIsDestination = false;
        var m = NPCsManager.Instance.GetWaypointManager(waypointManagerID);
        if(currentWaypoint == null)
            currentWaypoint = m.GetClosestWaypoint(transform.position);
        else if(currentWaypoint.transform == null)
            currentWaypoint = m.GetClosestWaypoint(transform.position);
        else
            currentWaypoint = m.GetNextWaypoint(currentWaypoint);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
    }

    public void SetPrevWaypointDestination()
    {
        if (!HasWaypointRoute())
        {
            return;
        }

        Debug.Log("Set Previous Waypoint Destination");
        playerIsDestination = false;
        var m = NPCsManager.Instance.GetWaypointManager(waypointManagerID);
        if (currentWaypoint == null)
            currentWaypoint = m.GetClosestWaypoint(transform.position);
        else if (currentWaypoint.transform == null)
            currentWaypoint = m.GetClosestWaypoint(transform.position);
        else
            currentWaypoint = m.GetPreviousWaypoint(currentWaypoint);
        Vector3 destination = currentWaypoint.transform.position;
        agent.SetDestination(destination);
    }
    #endregion

    public void SetPlayerDestination()
    {
        playerIsDestination = true;
    }

    public bool NPCAtDestination()
    {
        if (agent.pathPending)
        {
            Debug.Log("[Destination Check] Path still pending");
            return false;
        }

        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("[Destination Check] Path is invalid");
            return false;
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.LogFormat("[Destination Check] Agent at destination [Remaining Dist: {0}; Stopping Dist: {1}]",
                agent.remainingDistance, agent.stoppingDistance);
            return true;
        }

        return false;
    }

    public void SetPositionOffset(float minRadius, float maxRadius)
    {
        Vector3 offset = Random.insideUnitCircle;
        float radius = Random.Range(minRadius, maxRadius);
        positionOffset = new Vector3(offset.x, 0f, offset.y) * radius;
    }

    public void StopMoving()
    {
        agent.updateRotation = true;
        stopped = true;
    }

    public void ResumeMoving()
    {
        stopped = false;
    }

    public void SetRunning(bool running)
    {
        Debug.Log("Running Set to " + running);
        this.running = running;
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

    #region [Attacks]
    public void PerformLightAttackLeft()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        anim.ResetTrigger("AttackLightRight");
        anim.ResetTrigger("AttackCombo");
        anim.ResetTrigger("AttackHeavy");
        anim.SetTrigger("AttackLightLeft");
    }

    public void PerformLightAttackRight()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        anim.ResetTrigger("AttackLightLeft");
        anim.ResetTrigger("AttackCombo");
        anim.ResetTrigger("AttackHeavy");
        anim.SetTrigger("AttackLightRight");
    }

    public void PerformAttackCombo()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        anim.ResetTrigger("AttackLightLeft");
        anim.ResetTrigger("AttackLightRight");
        anim.ResetTrigger("AttackHeavy");
        anim.SetTrigger("AttackCombo");
    }

    public void PerformHeavyAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        anim.ResetTrigger("AttackLightLeft");
        anim.ResetTrigger("AttackLightRight");
        anim.ResetTrigger("AttackCombo");
        anim.SetTrigger("AttackHeavy");
    }

    public void ReleaseAttack()
    {
        isAttacking = false;
    }

    #region [Hurtboxes]
    public void EnableLightAttackLeftHurtbox(int enable)
    {
        onLightAttack.Invoke();
        lightAttackLeftHurtbox.SetActive(enable > 0);
    }

    public void EnableLightAttackRightHurtbox(int enable)
    {
        onLightAttack.Invoke();
        lightAttackLeftHurtbox.SetActive(enable > 0);
    }

    public void EnableHeavyAttackHurtbox(int enable)
    {
        onHeavyttack.Invoke();
        heavyAttackHurtbox.SetActive(enable > 0);
    }
    #endregion
    #endregion

    #region [NPC States]
    #region[NPC State Setters]
    public void SetWaitState()
    {
        npcState = NPCState.Wait;
    }

    public void SetStartState()
    {
        currentWaypoint = null;
        npcState = NPCState.Start;
    }

    public void SetPassiveState()
    {
        agent.updateRotation = true;
        npcState = NPCState.Passive;
        onPassiveState.Invoke();
    }

    public void SetChaseState()
    {
        currentWaypoint = null;
        agent.updateRotation = true;
        npcState = NPCState.Chase;
        onChaseState.Invoke();
    }

    public void SetAttackState()
    {
        agent.updateRotation = false;
        currentWaypoint = null;
        npcState = NPCState.Attack;
    }

    public void SetDeadState()
    {
        agent.updateRotation = false;
        StopMoving();
        currentWaypoint = null;
        npcState = NPCState.Dead;
    }
    #endregion

    #region [NPC State Checks]
    public bool IsWaitState()
    {
        return npcState == NPCState.Wait;
    }

    public bool IsStartState()
    {
        return npcState == NPCState.Start;
    }

    public bool IsPassiveState()
    {
        return npcState == NPCState.Passive;
    }

    public bool IsChaseState()
    {
        return npcState == NPCState.Chase;
    }

    public bool IsAttackState()
    {
        return npcState == NPCState.Attack;
    }

    public bool IsCombatState()
    {
        return IsChaseState() || IsAttackState();
    }

    public bool IsDeadState()
    {
        return npcState == NPCState.Dead;
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
        health = Mathf.Max(health - damage, 0);

        Vector3 hitDir = hitForce;
        hitDir.y = 0f;
        hitDir.Normalize();
        float dot = Vector3.Dot(hitDir, transform.right);

        // TODO: Update to change direction and hit intensity
        PlayHurtReaction(
            hitboxType,
            (dot > 0f),
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
        switch (hitboxType)
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
        SetDeadState();
        anim.SetBool("DeadBack", true);
        onDead.Invoke();
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (IsCombatState() || IsDeadState())
                return;

            Gizmos.color = (IsCombatState()) ? Color.red : Color.yellow;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Vector3 pos = head.position + Vector3.up * 0.05f;
        Gizmos.DrawWireSphere(pos, detectionRange);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 npcPos;
        if (Application.isPlaying)
        {
            npcPos = transform.position + Vector3.up * agent.height * 0.5f;
        }
        else
        {
            npcPos = transform.position;
            npcPos += Vector3.up * GetComponent<NavMeshAgent>().height * 0.5f;
        }
        Gizmos.DrawWireSphere(npcPos, attackRange);
        Gizmos.DrawLine(
            npcPos,
            npcPos + transform.forward * attackRange);
        Gizmos.DrawLine(
            npcPos,
            npcPos + Quaternion.Euler(Vector3.up * attackAngle * 0.5f) * transform.forward * attackRange);
        Gizmos.DrawLine(
            npcPos,
            npcPos + Quaternion.Euler(-Vector3.up * attackAngle * 0.5f) * transform.forward * attackRange);

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
    }
}
