using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(string.Format("{0} entered volume {1}", name, other.gameObject.name));
        onTriggerEnter.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(string.Format("{0} exited volume {1}", name, other.gameObject.name));
        onTriggerExit.Invoke();
    }
}
