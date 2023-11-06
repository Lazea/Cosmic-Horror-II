using UnityEngine;

public class NPCProjectile : MonoBehaviour
{
    public float speed = 2f;
    public float angularSpeed = 12f;
    public float aimSmooth = 0.15f;
    [SerializeField]
    float smoothScale = 10f;
    bool halt;

    Rigidbody rb;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 vel = transform.forward;
        vel.y = 0f;
        vel *= speed;
        rb.velocity = vel;
        rb.angularVelocity = Random.insideUnitSphere * angularSpeed;

        ps = GetComponent<ParticleSystem>();
        Invoke(
            "HaltMovement",
            Mathf.Min(ps.main.duration, ps.main.duration - 0.5f));
    }

    void FixedUpdate()
    {
        if (halt)
        {
            Vector3 vel = Vector3.Slerp(
                rb.velocity,
                Vector3.zero,
                aimSmooth * smoothScale * 2f * Time.deltaTime);
            rb.velocity = vel;
        }
        else
        {
            if (GameManager.Instance.player == null)
                return;
            if (GameManager.Instance.player.IsDead)
                return;

            Vector3 disp = GameManager.Instance.player.transform.position;
            disp -= transform.position;
            Vector3 vel = Vector3.Slerp(
                rb.velocity,
                disp.normalized * speed,
                aimSmooth * smoothScale * Time.deltaTime);
            rb.velocity = vel;
        }
    }

    public void HaltMovement()
    {
        halt = true;
        ps.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(Application.isPlaying)
        {
            Gizmos.DrawLine(
            transform.position,
            transform.position + rb.velocity);
        }
        else
        {
            Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * speed);
        }
    }
}
