using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOGameEventSystem;

public class VolumeTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public BaseGameEvent TriggerEnterEvent;
    public UnityEvent onTriggerStay;
    public BaseGameEvent TriggerStayEvent;
    public UnityEvent onTriggerExit;
    public BaseGameEvent TriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(string.Format("{0} entered volume {1}", name, other.gameObject.name));
        onTriggerEnter.Invoke();

        if (TriggerEnterEvent != null)
            TriggerEnterEvent.Raise();
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke();

        if (TriggerStayEvent != null)
            TriggerStayEvent.Raise();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(string.Format("{0} exited volume {1}", name, other.gameObject.name));
        onTriggerExit.Invoke();

        if (TriggerExitEvent != null)
            TriggerExitEvent.Raise();
    }
}
