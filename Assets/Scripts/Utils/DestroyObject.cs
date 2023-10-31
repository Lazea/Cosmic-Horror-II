using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [Tooltip("If less than or equal to 0.0 then the object lives forever otherwise it does after" +
        "the set amount of lifetime.")]
    public float lifetime;

    private void Awake()
    {
        if(lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    public void DestroyThisObjectWithDelay(float delay)
    {
        Destroy(gameObject, delay);
    }
}
