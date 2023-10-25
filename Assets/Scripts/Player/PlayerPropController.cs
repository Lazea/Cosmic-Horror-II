using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPropController : MonoBehaviour
{
    [Header("Throw Force")]
    public float throwForce;
    public UnityEvent onThrow;

    [Header("Prop")]
    public BaseProp equiptProp;
    public Collider propHoldingColliderLarge;
    public Collider propHoldingColliderSmall;

    [Header("Points")]
    public Transform playerHandOneHandedProps;
    public Transform playerHandTwoHandedProps;
    public Transform playerHandMediumProps;
    public Transform largePropDropPoint;
    public Transform smallPropDropPoint;
    public Transform propThrowPoint;

    [Header("Attacking")]
    public LayerMask attackMask;
    public UnityEvent onAttack;
    public UnityEvent<BaseProp> onAttackHit = new UnityEvent<BaseProp>();
    public UnityEvent onAttackMiss;

    //[Header("Blocking")]
    [HideInInspector]
    public UnityEvent onBlock;

    PropSettings propSettings
    {
        get { return GameManager.Instance.settings.propSettings; }
    }

    // Components
    Animator anim;
    PlayerCharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<PlayerCharacterController>();

        EnablePropHoldingCollider(false);
    }

    private void OnDisable()
    {
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("AttackHeavy");
        anim.SetBool("Blocking", false);
        anim.ResetTrigger("Block");
        anim.ResetTrigger("Equipt");
        anim.ResetTrigger("Throw");

        anim.SetFloat("Speed", 0f);
        anim.SetFloat("SpeedX", 0f);
        anim.SetFloat("SpeedY", 0f);
        anim.SetFloat("LookX", 0f);
        anim.SetFloat("LookY", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.IsRunning() && anim.GetBool("Blocking"))
            BlockEnd();
        else if(cc.IsClimbing() && anim.GetBool("Blocking"))
            BlockEnd();
    }


    private void FixedUpdate()
    {
        EnablePropHoldingCollider(equiptProp != null);
    }

    void EnablePropHoldingCollider(bool enable)
    {
        if(enable)
        {
            if (equiptProp != null)
            {
                switch (equiptProp.propType)
                {
                    case PropType.Medium:
                        propHoldingColliderLarge.enabled = true;
                        propHoldingColliderSmall.enabled = false;
                        return;
                    case PropType.Heavy:
                        propHoldingColliderLarge.enabled = true;
                        propHoldingColliderSmall.enabled = false;
                        return;
                    default:
                        propHoldingColliderLarge.enabled = false;
                        propHoldingColliderSmall.enabled = true;
                        return;
                }
            }
        }

        propHoldingColliderLarge.enabled = false;
        propHoldingColliderSmall.enabled = false;
    }

    #region [Equipt and Drop]
    public void EquiptProp(BaseProp prop)
    {
        if (cc.IsClimbing())
            return;

        if (equiptProp != null)
        {
            DropProp();
        }

        switch (prop.propType)
        {
            case PropType.OneHanded:
                prop.transform.parent = playerHandOneHandedProps;
                break;
            case PropType.TwoHanded:
                prop.transform.parent = playerHandTwoHandedProps;
                break;
            case PropType.Medium:
                prop.transform.parent = playerHandMediumProps;
                break;
        }
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;
        prop.isHeld = true;

        equiptProp = prop;
        EnablePropHoldingCollider(true);

        anim.SetTrigger("Equipt");
        anim.SetBool("Blocking", false);
    }

    public bool UnequiptProp()
    {
        if (equiptProp == null)
        {
            Debug.Log("No prop to unequipt");
            return false;
        }

        EnablePropHoldingCollider(false);

        equiptProp.transform.parent = GameManager.Instance.propsContainer;
        foreach (Collider coll in equiptProp.Colliders)
            coll.enabled = true;
        equiptProp.RB.isKinematic = false;
        equiptProp.RB.interpolation = RigidbodyInterpolation.Interpolate;
        equiptProp.RB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        equiptProp.isHeld = false;

        equiptProp = null;
        return true;
    }

    public void DropProp()
    {
        if (!this.enabled)
            return;

        if (cc.IsClimbing())
            return;

        var _prop = equiptProp;
        if (!UnequiptProp())
        {
            return;
        }

        Debug.Log(string.Format("Drop {0}", _prop.name));

        if(_prop.propType == PropType.Medium || _prop.propType == PropType.Heavy)
        {
            _prop.transform.position = largePropDropPoint.position;
            _prop.transform.rotation = largePropDropPoint.rotation;
        }
        else
        {
            _prop.transform.position = smallPropDropPoint.position;
            _prop.transform.rotation = smallPropDropPoint.rotation;
        }
        _prop.RB.velocity = Vector3.zero;
        _prop.RB.angularVelocity = Vector3.zero;

        anim.SetBool("Blocking", false);
    }
    #endregion

    #region [Prop Actions]
    public void ThrowProp()
    {
        if (!this.enabled)
            return;

        if (cc.IsClimbing())
            return;

        if (cc.IsRunning())
            return;

        var _prop = equiptProp;
        if (!UnequiptProp())
        {
            return;
        }

        Debug.Log(string.Format("Throw {0}", _prop.name));

        _prop.transform.position = propThrowPoint.position;
        _prop.transform.rotation = propThrowPoint.rotation;
        _prop.RB.velocity = Vector3.zero;
        float _throwForce = (_prop.RB.mass >= 6f) ? throwForce * 1.75f : throwForce;
        _prop.RB.AddForce(_prop.transform.forward * _throwForce, ForceMode.Impulse);
        _prop.RB.angularVelocity = _prop.transform.forward * Random.Range(-1f, 1f) * 4f;
        _prop.isThrown = true;

        anim.SetTrigger("Throw");
        anim.SetBool("Blocking", false);
    }

    #region [Prop Attacking]
    public void Attack()
    {
        if (!this.enabled)
            return;

        if (cc.IsClimbing())
            return;

        if (cc.IsRunning())
            return;

        if (equiptProp == null)
            return;

        var animState = anim.GetCurrentAnimatorStateInfo(0);
        bool canAttack = false;
        if (animState.IsTag("Idle") ||
           animState.IsTag("AttackContinue"))
        {
            canAttack = true;
        }
        else if (animState.IsTag("Attack") && animState.normalizedTime >= 0.2f)
        {
            canAttack = true;
        }
        else if (animState.IsTag("AttackHeavy") && animState.normalizedTime >= 0.55f)
        {
            canAttack = true;
        }

        if (canAttack)
        {
            if (equiptProp.propType == PropType.Medium)
                anim.SetTrigger("AttackHeavy");
            else
                anim.SetTrigger("Attack");
        }
    }

    public void AttackStart()
    {
        onAttack.Invoke();
    }

    public void PerformAttack1()
    {
        if (equiptProp == null)
            return;

        Vector3 forceDir = -Camera.main.transform.right;
        forceDir += Camera.main.transform.up * 0.1f;
        PerformAttackRaycast(
            equiptProp.damage,
            GetAttackRange(equiptProp),
            forceDir * GetAttackForce(equiptProp));
    }

    public void PerformAttack2()
    {
        if (equiptProp == null)
            return;

        Vector3 forceDir = Camera.main.transform.right;
        forceDir += Camera.main.transform.up * 0.1f;
        PerformAttackRaycast(
            equiptProp.damage,
            GetAttackRange(equiptProp),
            forceDir * GetAttackForce(equiptProp));
    }

    public void PerformHeavyAttack1()
    {
        if (equiptProp != null)
            return;

        Vector3 forceDir = -Camera.main.transform.right;
        forceDir += Camera.main.transform.up * 0.1f;
        PerformAttackRaycast(
            equiptProp.damage,
            GetAttackRange(equiptProp),
            forceDir * GetAttackForce(equiptProp));
    }

    float GetAttackRange(BaseProp prop)
    {
        float range = 0f;
        switch (prop.propAttackRange)
        {
            case BaseProp.PropAttackRange.Short:
                range = propSettings.AttackShortRange;
                break;
            case BaseProp.PropAttackRange.Long:
                range = propSettings.AttackShortRange;
                break;
            default:
                range = 1f;
                break;
        }

        return range;
    }

    float GetAttackForce(BaseProp prop)
    {
        float force = 0f;
        switch (prop.propAttackForce)
        {
            case BaseProp.PropAttackForce.Light:
                force = propSettings.attackLightForce;
                break;
            case BaseProp.PropAttackForce.Heavy:
                force = propSettings.attackHeavyForce;
                break;
            default:
                force = 15f;
                break;
        }

        return force;
    }

    bool PerformAttackRaycast(
        int attackDamage,
        float attackRange,
        Vector3 attackForce)
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(
            ray,
            out hit,
            attackRange,
            attackMask))
        {
            var hitDamageable = hit.collider.GetComponent<IDamageable>();
            if(hitDamageable != null)
            {
                hitDamageable.DealDamage(
                    attackDamage,
                    attackForce,
                    hit.point,
                    gameObject);
            }
            else if(hit.collider.attachedRigidbody != null)
            {
                hit.collider.attachedRigidbody.AddForceAtPosition(
                    attackForce,
                    hit.point,
                    ForceMode.Impulse);
            }

            Debug.Log(
                string.Format("Player Attack hit {0} with {1} damage and {2}:{3} force",
                hit.collider.name, attackDamage, attackForce, attackForce.magnitude));

            onAttackHit.Invoke(equiptProp);

            equiptProp.durability--;
            if (equiptProp.durability <= 0)
                DestroyEquiptProp();

            return true;
        }
        else
        {
            onAttackMiss.Invoke();
            return false;
        }
    }
    #endregion

    public void BlockStart()
    {
        if (!this.enabled)
            return;

        if (cc.IsClimbing())
            return;

        if (cc.IsRunning())
            return;

        if (equiptProp == null)
            return;

        var animState = anim.GetCurrentAnimatorStateInfo(0);
        if (animState.IsTag("Attack") ||
            animState.IsTag("AttackContinue") ||
            animState.IsName("Equipt") ||
            animState.IsName("Throw"))
            return;
        anim.SetBool("Blocking", true);
    }

    public void Block()
    {
        if (!this.enabled)
            return;

        if (equiptProp == null)
            return;

        if (!anim.GetBool("Blocking"))
            return;

        anim.SetTrigger("Block");
        onBlock.Invoke();
        equiptProp.durability--;
        if (equiptProp.durability <= 0)
            DestroyEquiptProp();
    }

    public void BlockEnd()
    {
        if (equiptProp == null)
            return;

        anim.SetBool("Blocking", false);
    }

    public void DestroyEquiptProp()
    {
        var _prop = equiptProp;
        equiptProp = null;

        anim.ResetTrigger("AttackHeavy");
        anim.ResetTrigger("Attack");

        _prop.DestroyObject();
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.blue;
        var cameraTransform = Camera.main.transform;
        Gizmos.DrawWireSphere(
            cameraTransform.position + cameraTransform.forward * propSettings.AttackShortRange,
            0.25f);
        Gizmos.DrawLine(
            cameraTransform.position,
            cameraTransform.position + cameraTransform.forward * propSettings.AttackShortRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            cameraTransform.position + cameraTransform.forward * propSettings.AttackLongRange,
            0.25f);
        Gizmos.DrawLine(
            cameraTransform.position,
            cameraTransform.position + cameraTransform.forward * propSettings.AttackLongRange);
    }
}
