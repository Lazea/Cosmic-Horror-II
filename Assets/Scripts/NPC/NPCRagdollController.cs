using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCRagdollController : MonoBehaviour
{
    public GameObject pelvis;
    Rigidbody[] rigidBodies;
    Collider[] colliders;
    CharacterJoint[] joints;

    public Behaviour[] components;
    Collider npcCollider;
    Animator anim;

    public struct BoneTransform
    {
        public Transform bone;
        public Vector3 previousPosition;
        public Vector3 position;
        public Vector3 previousRotation;
        public Vector3 rotation;
        public float deltaTime;

        public Vector3 GetVelocity()
        {
            return (position - previousPosition) / deltaTime;
        }

        public Vector3 GetAngularVelocity()
        {
            return (rotation - previousRotation) / deltaTime;
        }
    }
    BoneTransform[] bones;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = pelvis.GetComponentsInChildren<Rigidbody>();
        colliders = pelvis.GetComponentsInChildren<Collider>();
        joints = pelvis.GetComponentsInChildren<CharacterJoint>();

        bones = new BoneTransform[rigidBodies.Length];
        for (int i = 0; i < rigidBodies.Length; i++)
            bones[i].bone = rigidBodies[i].transform;
        UpdateBoneVelocities();

        npcCollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        anim.SetFloat("DeathBlend", Random.Range(0f, 1f));
    }

    void FixedUpdate()
    {
        UpdateBoneVelocities();
    }

    void UpdateBoneVelocities()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].previousPosition = bones[i].position;
            bones[i].position = bones[i].bone.position;

            bones[i].previousRotation = bones[i].rotation;
            bones[i].rotation = bones[i].bone.rotation.eulerAngles;

            bones[i].deltaTime = Time.fixedDeltaTime;
        }
    }

    [ContextMenu("Enable Ragdoll")]
    public void EnableRagdoll()
    {
        Invoke("EnableRG", Random.Range(0.01f, 0.3f));
        //anim.enabled = false;
        //foreach(var c in components)
        //{
        //    c.enabled = false;
        //}
        //npcCollider.enabled = false;

        //foreach(var rb in rigidBodies)
        //{
        //    rb.isKinematic = false;
        //    rb.useGravity = true;
        //}

        //for(int i = 0; i < rigidBodies.Length; i++)
        //{
        //    rigidBodies[i].velocity = bones[i].GetVelocity();
        //    rigidBodies[i].angularVelocity = bones[i].GetAngularVelocity();
        //}

        //foreach (var j in joints)
        //{
        //    j.enableProjection = true;
        //}
    }

    void EnableRG()
    {
        anim.enabled = false;
        foreach (var c in components)
        {
            c.enabled = false;
        }
        npcCollider.enabled = false;

        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].velocity = bones[i].GetVelocity();
            rigidBodies[i].angularVelocity = bones[i].GetAngularVelocity();
        }

        foreach (var j in joints)
        {
            j.enableProjection = true;
        }
    }

    [ContextMenu("Disable Ragdoll")]
    public void DisableRagdoll()
    {
        foreach (var j in joints)
        {
            j.enableProjection = false;
        }

        foreach (var rb in rigidBodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        anim.enabled = true;
        foreach (var c in components)
        {
            c.enabled = true;
        }
        npcCollider.enabled = true;
    }
}
