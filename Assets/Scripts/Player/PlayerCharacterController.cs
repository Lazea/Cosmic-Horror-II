using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    Vector3 velocity;

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
            float fallSpeed = rb.velocity.y;
            velocity = rb.velocity;
            velocity.y = 0f;

            Vector3 moveDir = transform.TransformVector(
                new Vector3(moveInput.x, 0f, moveInput.y));
            float speed = (run && moveInput.y > 0.2f) ?
                playerSettings.runSpeed :
                playerSettings.walkSpeed;
            velocity = Vector3.Lerp(
                velocity,
                moveDir * speed,
                playerSettings.acceleration);
            velocity.y = fallSpeed;

            rb.velocity = velocity;
        }

        run = false;
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

    public void Run()
    {
        run = true;
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
    }
}
