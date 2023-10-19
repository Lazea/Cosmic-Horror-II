using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Ladder : MonoBehaviour, IInteractable
{
    GameObject actor;
    bool actorReadyToClimb;

    [Header("Movement Stats")]
    public float climbSpeed;
    public float mountTime;

    [Header("Mount Points")]
    public Transform topMountPoint;
    public Transform bottomMountPoint;
    public Transform topDismountPoint;
    public Transform bottomDismountPoint;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Components
    Controls.GameplayActions controls;

    private void Awake()
    {
        controls = new Controls().Gameplay;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!actorReadyToClimb)
            return;

        Vector3 velocity = transform.up * climbSpeed;
        velocity *= controls.Movement.ReadValue<Vector2>().y;
        actor.transform.position += velocity * Time.deltaTime;

        var coll = actor.GetComponent<CapsuleCollider>();
        Vector3 actorPoint = actor.transform.position;
        actorPoint += Vector3.down * coll.height * 0.5f;

        if(actorPoint.y > topMountPoint.position.y)
        {
            Vector3 dismountPoint = topDismountPoint.position;
            dismountPoint += Vector3.up * coll.height * 0.5f;
            StartCoroutine(DismountActorFromLadder(dismountPoint, actor));
            actor = null;
        }
        if(actorPoint.y < bottomMountPoint.position.y)
        {
            Vector3 dismountPoint = bottomDismountPoint.position;
            dismountPoint += Vector3.up * coll.height * 0.5f;
            StartCoroutine(DismountActorFromLadder(dismountPoint, actor));
            actor = null;
        }
    }

    public void Interact(GameObject actor)
    {
        Vector3 mountPoint;
        var coll = actor.GetComponent<CapsuleCollider>();
        Vector3 actorPoint = actor.transform.position;
        actorPoint += Vector3.down * coll.height * 0.5f;
        if (actorPoint.y < bottomMountPoint.position.y)
        {
            mountPoint = bottomMountPoint.position + transform.up * 0.1f;
        }
        else if(actorPoint.y > topMountPoint.position.y)
        {
            mountPoint = topMountPoint.position - transform.up * 0.5f;
        }
        else
        {
            var plane = new Plane(Vector3.up, actorPoint);
            Ray ray = new Ray(
                topMountPoint.position,
                bottomMountPoint.position - topMountPoint.position);
            float d;
            plane.Raycast(ray, out d);

            mountPoint = ray.origin + ray.direction * d;
        }

        mountPoint += Vector3.up * coll.height * 0.5f;
        this.actor = actor;
        StartCoroutine(MountActorToLadder(mountPoint, actor));
    }

    IEnumerator MountActorToLadder(Vector3 mountPoint, GameObject actor)
    {
        actor.GetComponent<Player>().DisablePlayer();

        float t = 0f;
        Vector3 startPoint = actor.transform.position;
        Quaternion startRot = actor.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(
            new Vector3(-transform.forward.x, 0f, -transform.forward.z));
        while (true)
        {
            if (t >= mountTime)
                break;

            actor.transform.position = Vector3.Lerp(
                startPoint,
                mountPoint,
                t / mountTime);
            actor.transform.rotation = Quaternion.Lerp(
                startRot,
                targetRot,
                t / (mountTime * 0.75f));

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        actorReadyToClimb = true;
    }

    IEnumerator DismountActorFromLadder(Vector3 dismountPoint, GameObject actor)
    {
        actorReadyToClimb = false;

        float t = 0f;
        Vector3 startPoint = actor.transform.position;
        while (true)
        {
            if (t >= mountTime)
                break;

            actor.transform.position = Vector3.Lerp(
                startPoint,
                dismountPoint,
                t / mountTime);

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        actor.GetComponent<Player>().EnablePlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (topMountPoint != null)
            Gizmos.DrawWireSphere(topMountPoint.transform.position, 0.1f);
        if (bottomMountPoint != null)
            Gizmos.DrawWireSphere(bottomMountPoint.transform.position, 0.1f);
        if (topMountPoint != null && bottomMountPoint != null)
            Gizmos.DrawLine(
                topMountPoint.transform.position,
                bottomMountPoint.transform.position);

        Gizmos.color = Color.red;
        if (topDismountPoint != null)
            Gizmos.DrawWireSphere(topDismountPoint.transform.position, 0.1f);
        if (bottomDismountPoint != null)
            Gizmos.DrawWireSphere(bottomDismountPoint.transform.position, 0.1f);
    }
}
