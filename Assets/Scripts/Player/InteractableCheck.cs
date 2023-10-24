using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SOGameEventSystem.Events;

public class InteractableCheck : MonoBehaviour
{
    [Header("Interactables Stats")]
    public float checkRadius;
    public AnimationCurve checkDistanceCurve;
    public float checkDistance;
    float _checkDistance;
    public LayerMask mask;
    public LayerMask coverMask;
    RaycastHit hit;

    bool hasSelection;
    bool isPickingUp;
    public IProp selectedProp;
    public IInteractable selectedInteractable;
    bool quitting;

    Transform cameraTransform
    {
        get { return Camera.main.transform; }
    }

    [Header("Events")]
    public UnityEvent<BaseProp> onPropPickup = new UnityEvent<BaseProp>();
    public IntGameEvent KeyPickup;
    public UnityEvent onKeyPickup = new UnityEvent();
    public UnityEvent<HealthPickup> onHealthPickup = new UnityEvent<HealthPickup>();

    // Components
    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnDisable()
    {
        StopCoroutine("PickupProp");

        if (quitting)
            return;

        selectedProp = null;
        selectedInteractable = null;
        if(UIManager.Instance != null)
            UIManager.Instance.HideInteractionPrompt();
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }

    // Update is called once per frame
    void Update()
    {
        selectedProp = null;
        selectedInteractable = null;
        hasSelection = false;

        float angle = Vector3.Angle(Vector3.up, Camera.main.transform.forward);
        _checkDistance = checkDistanceCurve.Evaluate(angle);
        _checkDistance *= checkDistance;
        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward);
        if(Physics.SphereCast(
            ray,
            checkRadius,
            out hit,
            _checkDistance,
            mask))
        {
            Vector3 hitDisp = hit.point - cameraTransform.position;
            ray = new Ray(
                cameraTransform.position,
                hitDisp);
            if(!Physics.Raycast(
                ray,
                hitDisp.magnitude + 0.002f,
                coverMask))
            {
                if (hit.collider.tag == "Prop")
                {
                    var healthPickup = hit.collider.GetComponent<HealthPickup>();
                    if (healthPickup != null)
                    {
                        if (player.IsHurt)
                        {
                            UIManager.Instance.ShowInteractionPrompt("[E] to Pick Up");
                            selectedProp = hit.collider.GetComponent<IProp>();
                        }
                        else
                        {
                            UIManager.Instance.ShowInteractionPrompt("Health is Full");
                        }
                    }
                    else
                    {
                        UIManager.Instance.ShowInteractionPrompt("[E] to Pick Up");
                        selectedProp = hit.collider.GetComponent<IProp>();
                    }
                    hasSelection = true;
                }
                //else if (hit.collider.tag == "LargeProp")
                //{
                //    UIManager.Instance.ShowInteractionPrompt("Hold [E] to Move");
                //    hasSelection = true;
                //}
                else if (hit.collider.tag == "Interactable")
                {
                    UIManager.Instance.ShowInteractionPrompt("[E] to Interact");
                    selectedInteractable = hit.collider.GetComponent<IInteractable>();
                    hasSelection = true;
                }
            }
        }

        if(!hasSelection)
            UIManager.Instance.HideInteractionPrompt();
    }

    public void Interact()
    {
        if (!this.enabled)
            return;

        if (isPickingUp)
        {
            Debug.Log("Still picking up prop");
            return;    
        }

        if(selectedInteractable != null)
        {
            Debug.Log(string.Format(
                "Interact with {0}",
                selectedInteractable.GetGameObject().name));
            selectedInteractable.Interact(gameObject);
        }

        if (selectedProp != null)
        {
            isPickingUp = true;
            Debug.Log(string.Format(
                "Interact with {0}",
                selectedProp.GetGameObject().name));
            StartCoroutine(PickupProp(selectedProp));
            return;
        }

        Debug.Log("Nothing to interact with");
    }

    IEnumerator PickupProp(IProp prop)
    {
        foreach (Collider coll in prop.Colliders)
            coll.enabled = false;
        prop.RB.isKinematic = true;
        prop.RB.interpolation = RigidbodyInterpolation.None;
        prop.RB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        prop.RB.velocity = Vector3.zero;
        prop.RB.angularVelocity = Vector3.zero;

        while (Vector3.Distance(
            prop.GetGameObject().transform.position,
            transform.position) >= 0.1f)
        {
            prop.GetGameObject().transform.position = Vector3.Lerp(
                prop.GetGameObject().transform.position,
                transform.position,
                25f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        var propGObj = prop.GetGameObject();

        // Pickup Prop
        var baseProp = propGObj.GetComponent<BaseProp>();
        if(baseProp != null)
        {
            onPropPickup.Invoke(baseProp);
            isPickingUp = false;
        }

        // Pickup Key
        var keyPickup = propGObj.GetComponent <KeyPickup>();
        if (keyPickup != null)
        {
            KeyPickup.Raise(keyPickup.keyID);
            onKeyPickup.Invoke();
            isPickingUp = false;

            Destroy(keyPickup.gameObject);
        }

        // Pickup Health
        var healthPickup = propGObj.GetComponent<HealthPickup>();
        if (healthPickup != null)
        {
            onHealthPickup.Invoke(healthPickup);
            isPickingUp = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (hit.collider != null) ? Color.green : Color.red;

        float distance = checkDistance;
        if (Application.isPlaying)
            distance = _checkDistance;
        Gizmos.DrawWireSphere(
            cameraTransform.position,
            checkRadius);
        Gizmos.DrawWireSphere(
            cameraTransform.position + cameraTransform.forward * distance,
            checkRadius);
        Gizmos.DrawLine(
            cameraTransform.position,
            cameraTransform.position + cameraTransform.forward * distance);
    }
}
