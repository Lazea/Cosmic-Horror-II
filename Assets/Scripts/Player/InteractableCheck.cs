using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableCheck : MonoBehaviour
{
    [Header("Interactables Stats")]
    public float checkRadius;
    public float checkDistance;
    public LayerMask mask;
    public LayerMask coverMask;
    RaycastHit hit;

    bool hasSelection;
    bool isPickingUp;
    public BaseProp selectedProp;

    Transform cameraTransform
    {
        get { return Camera.main.transform; }
    }

    [Header("Events")]
    public UnityEvent<BaseProp> onPropPickup = new UnityEvent<BaseProp>();

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        selectedProp = null;
        hasSelection = false;

        Ray ray = new Ray(
            cameraTransform.position,
            cameraTransform.forward);
        if(Physics.SphereCast(
            ray,
            checkRadius,
            out hit,
            checkDistance,
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
                    UIManager.Instance.ShowInteractionPrompt("[E] to Pick Up");
                    selectedProp = hit.collider.GetComponent<BaseProp>();
                    hasSelection = true;
                }
                else if (hit.collider.tag == "LargeProp")
                {
                    UIManager.Instance.ShowInteractionPrompt("Hold [E] to Move");
                    hasSelection = true;
                }
                else if (hit.collider.tag == "Interactable")
                {
                    UIManager.Instance.ShowInteractionPrompt("Hold [E] to Interact");
                    hasSelection = true;
                }
            }
        }

        if(!hasSelection)
            UIManager.Instance.HideInteractionPrompt();
    }

    public void Interact()
    {
        if (isPickingUp)
        {
            Debug.Log("Still picking up prop");
            return;    
        }

        if (selectedProp == null)
        {
            Debug.Log("Nothing to interact with");
            return;
        }

        isPickingUp = true;
        StartCoroutine(PickupProp(selectedProp));
        Debug.Log(string.Format("Interact with {0}", selectedProp.name));
    }

    IEnumerator PickupProp(BaseProp prop)
    {
        foreach(Collider coll in prop.colls)
            coll.enabled = false;
        prop.rb.isKinematic = true;
        prop.rb.interpolation = RigidbodyInterpolation.None;
        prop.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        prop.rb.velocity = Vector3.zero;
        prop.rb.angularVelocity = Vector3.zero;

        while (Vector3.Distance(prop.transform.position, transform.position) >= 0.1f)
        {
            prop.transform.position = Vector3.Lerp(
                prop.transform.position,
                transform.position,
                25f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        onPropPickup.Invoke(prop);
        isPickingUp = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (hit.collider != null) ? Color.green : Color.red;
        Gizmos.DrawWireSphere(
            cameraTransform.position,
            checkRadius);
        Gizmos.DrawWireSphere(
            cameraTransform.position + cameraTransform.forward * checkDistance,
            checkRadius);
        Gizmos.DrawLine(
            cameraTransform.position,
            cameraTransform.position + cameraTransform.forward * checkDistance);
    }
}
