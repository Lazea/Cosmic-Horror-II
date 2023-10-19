using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour, IProp
{
    Rigidbody rb;
    public Rigidbody RB { get { return rb; } }

    Collider[] colliders;
    public Collider[] Colliders { get { return colliders; } }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
