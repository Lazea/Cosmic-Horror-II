using System.Collections;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    // Input
    [SerializeField]
    Vector2 moveInput;
    [SerializeField]
    Vector2 lookInput;
    [SerializeField]
    bool run;
    [SerializeField]
    bool isRunning;
    [SerializeField]
    bool isClimbing;

    Vector3 velocity;

    [Header("Slope Speed Curve")]
    public AnimationCurve slopeSpeedCurve;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.45f;
    public float groundCheckDistance = 0.55f;
    public LayerMask groundMask;
    RaycastHit groundHit;
    [SerializeField]
    bool isGrounded;

    [Header("Animation Smoothing")]
    public float moveAnimSmooth;
    public float lookAnimSmooth;

    [Header("Ledge Climb")]
    public Vector3 ledgeCheckPoint;
    public float ledgeCheckDistance;
    public float ledgeClimbTime;

    // Components
    PlayerSettings playerSettings
    {
        get { return GameManager.Instance.settings.playerSettings; }
    }
    Rigidbody rb;
    CapsuleCollider collider;
    PhysicMaterial physicsMaterial;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        physicsMaterial = new PhysicMaterial();
        collider.material = physicsMaterial;

        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();

        UpdatePhysicsMaterial();

        if (isGrounded)
        {
            ComputeVelocity();
            rb.velocity = velocity;
        }
        else
        {
            isRunning = false;
        }
    }

    bool CheckGround()
    {
        Ray ray = new Ray(
            transform.position,
            Vector3.down);
        return Physics.SphereCast(
            ray,
            groundCheckRadius,
            out groundHit,
            groundCheckDistance,
            groundMask);
    }

    void ComputeVelocity()
    {
        velocity = rb.velocity;
        float fallSpeed = rb.velocity.y;
        velocity.y = 0f;

        Vector3 moveDir = transform.TransformVector(
            new Vector3(moveInput.x, 0f, moveInput.y));

        isRunning = (run && moveInput.y > 0.2f);
        float speed = (isRunning) ?
            playerSettings.runSpeed :
            playerSettings.walkSpeed;

        // Adjust speed based on slope
        float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);
        slopeAngle *= Mathf.Abs(
            Vector3.Dot(
                transform.forward,
                new Vector3(groundHit.normal.x, 0f, groundHit.normal.z).normalized));
        speed *= slopeSpeedCurve.Evaluate(
            slopeAngle / 50f);

        velocity = Vector3.Lerp(
            velocity,
            moveDir * speed,
            playerSettings.acceleration);

        Debug.DrawLine(
                groundHit.point,
                groundHit.point + velocity,
                Color.blue);

        velocity.y = fallSpeed;
    }

    void UpdatePhysicsMaterial()
    {
        float staticFriction = (moveInput.magnitude > 0f) ?
            playerSettings.dynamicPhysicsMaterial.staticFriction :
            playerSettings.staticPhysicsMaterial.staticFriction;
        if (!isGrounded)
            staticFriction = playerSettings.dynamicPhysicsMaterial.staticFriction;

        float dynamicFriction = (moveInput.magnitude > 0f) ?
            playerSettings.dynamicPhysicsMaterial.dynamicFriction :
            playerSettings.staticPhysicsMaterial.dynamicFriction;
        if (!isGrounded)
            dynamicFriction = playerSettings.dynamicPhysicsMaterial.dynamicFriction;

        physicsMaterial.staticFriction = Mathf.Lerp(
            physicsMaterial.staticFriction,
            staticFriction,
            0.75f);
        physicsMaterial.dynamicFriction = Mathf.Lerp(
            physicsMaterial.dynamicFriction,
            dynamicFriction,
            0.75f);
        physicsMaterial.frictionCombine = (moveInput.magnitude > 0f || !isGrounded) ?
            playerSettings.dynamicPhysicsMaterial.frictionCombine :
            playerSettings.staticPhysicsMaterial.frictionCombine;

        collider.material = physicsMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        // Animate movement lean
        Vector2 animMove = new Vector2(
            anim.GetFloat("SpeedX"),
            anim.GetFloat("SpeedY"));
        animMove = Vector2.Lerp(
            animMove,
            moveInput,
            moveAnimSmooth * Time.deltaTime);

        anim.SetFloat("SpeedX", animMove.x);
        anim.SetFloat("SpeedY", animMove.y);

        // Animate movement
        float targetAnimSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude /
            playerSettings.runSpeed;
        float animSpeed = Mathf.Lerp(
            anim.GetFloat("Speed"),
            targetAnimSpeed,
            moveAnimSmooth * Time.deltaTime);
        anim.SetFloat("Speed", animSpeed);

        // Animate Look Lean
        Vector2 animLook = new Vector2(
            anim.GetFloat("LookX"),
            anim.GetFloat("LookY"));
        animLook = Vector2.Lerp(
            animLook,
            lookInput,
            lookAnimSmooth * Time.deltaTime);

        anim.SetFloat("LookX", animLook.x);
        anim.SetFloat("LookY", animLook.y);
    }

    public void Move(Vector2 move)
    {
        moveInput = move;
    }

    public void Climb()
    {
        Ray ray = new Ray(
            transform.TransformPoint(ledgeCheckPoint),
            Vector3.down);
        RaycastHit ledgeHit;
        if(Physics.Raycast(
            ray,
            out ledgeHit,
            ledgeCheckDistance))
        {
            ray = new Ray(
                ledgeHit.point - Vector3.up * 0.01f,
                Vector3.up);
            if (!Physics.Raycast(
                ray,
                collider.height))
            {
                Vector3 point = ledgeHit.point + Vector3.up * collider.height * 0.5f;
                StartCoroutine(ClimbLedge(point));
            }
        }
    }

    IEnumerator ClimbLedge(Vector3 point)
    {
        collider.enabled = false;
        rb.isKinematic = true;
        isClimbing = true;
        isRunning = false;
        run = false;

        float t = 0f;

        while (true)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                point,
                t / ledgeClimbTime);

            t += Time.fixedDeltaTime;
            if (t > ledgeClimbTime)
                break;

            if (Vector3.Distance(transform.position, point) <= 0.05f)
                break;

            yield return new WaitForFixedUpdate();
        }

        collider.enabled = true;
        rb.isKinematic = false;
        isClimbing = false;
    }

    public bool IsClimbing()
    {
        return isClimbing;
    }

    public void Run(bool run)
    {
        if (isClimbing)
            return;

        this.run = run;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public void AnimateLookLean(Vector2 look)
    {
        lookInput = look.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (isGrounded) ? Color.green : Color.red;
        Gizmos.DrawWireSphere(
            transform.position + Vector3.down * groundCheckDistance,
            groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            transform.TransformPoint(ledgeCheckPoint),
            transform.TransformPoint(ledgeCheckPoint) - Vector3.up * ledgeCheckDistance);
    }
}
