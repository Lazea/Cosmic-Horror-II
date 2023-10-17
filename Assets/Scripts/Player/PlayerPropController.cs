using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPropController : MonoBehaviour
{
    [Header("Throw Force")]
    public float throwForce;

    [Header("Prop")]
    public BaseProp equiptProp;

    [Header("Points")]
    public Transform playerHandOneHandedProps;
    public Transform playerHandTwoHandedProps;
    public Transform playerHandMediumProps;
    public Transform propDropPoint;
    public Transform propThrowPoint;

    // Components
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EquiptProp(BaseProp prop)
    {
        if (equiptProp != null)
        {
            DropProp();
        }

        switch(prop.propType)
        {
            case BaseProp.PropType.OneHanded:
                prop.transform.parent = playerHandOneHandedProps;
                break;
            case BaseProp.PropType.TwoHanded:
                prop.transform.parent = playerHandTwoHandedProps;
                break;
            case BaseProp.PropType.Medium:
                prop.transform.parent = playerHandMediumProps;
                break;
        }
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;
        prop.isHeld = true;

        equiptProp = prop;

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

        equiptProp.transform.parent = GameManager.Instance.propsContainer;
        foreach (Collider coll in equiptProp.colls)
            coll.enabled = true;
        equiptProp.rb.isKinematic = false;
        equiptProp.rb.interpolation = RigidbodyInterpolation.Interpolate;
        equiptProp.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        equiptProp.isHeld = false;

        equiptProp = null;
        return true;
    }

    public void DropProp()
    {
        var _prop = equiptProp;
        if (!UnequiptProp())
        {
            return;
        }

        Debug.Log(string.Format("Drop {0}", _prop.name));

        _prop.transform.position = propDropPoint.position;
        _prop.transform.rotation = propDropPoint.rotation;
        _prop.rb.velocity = _prop.transform.forward * 0.75f;
        _prop.rb.angularVelocity = Vector3.zero;

        anim.SetBool("Blocking", false);
    }

    public void ThrowProp()
    {
        var _prop = equiptProp;
        if (!UnequiptProp())
        {
            return;
        }

        Debug.Log(string.Format("Throw {0}", _prop.name));

        _prop.transform.position = propThrowPoint.position;
        _prop.transform.rotation = propThrowPoint.rotation;
        float _throwForce = (_prop.rb.mass >= 6f) ? throwForce * 1.75f : throwForce;
        _prop.rb.AddForce(_prop.transform.forward * _throwForce, ForceMode.Impulse);
        _prop.rb.angularVelocity = _prop.transform.forward * Random.Range(-1f, 1f) * 4f;

        anim.SetTrigger("Throw");
        anim.SetBool("Blocking", false);
    }

    public void Attack()
    {
        if (equiptProp == null)
            return;

        var animState = anim.GetCurrentAnimatorStateInfo(0);
        bool canAttack = false;
        if (animState.IsTag("Idle") ||
           animState.IsTag("AttackContinue"))
        {
            canAttack = true;
        }
        else if(animState.IsTag("Attack") && animState.normalizedTime >= 0.25f)
        {
            canAttack = true;
        }

        if (canAttack)
        {
            if (equiptProp.propType == BaseProp.PropType.Medium)
                anim.SetTrigger("AttackHeavy");
            else
                anim.SetTrigger("Attack");
        }
    }

    public void BlockStart()
    {
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
        if (equiptProp == null)
            return;

        if (!anim.GetBool("Blocking"))
            return;

        anim.SetTrigger("Block");
    }

    public void BlockEnd()
    {
        if (equiptProp == null)
            return;

        anim.SetBool("Blocking", false);
    }
}
