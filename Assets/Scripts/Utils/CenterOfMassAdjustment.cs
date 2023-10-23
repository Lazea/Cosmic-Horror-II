using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMassAdjustment : MonoBehaviour
{
    public Transform comOverride;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (comOverride != null)
            rb.centerOfMass = comOverride.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        if(!Application.isPlaying)
        {
            rb = GetComponent<Rigidbody>();

            if(comOverride != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(
                    comOverride.position,
                    0.15f);
            }
        }

        if (!enabled)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(
            transform.TransformPoint(rb.centerOfMass),
            0.1f);
    }
}
