using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProp
{
    public Rigidbody RB { get; }
    public Collider[] Colliders { get; }
    public GameObject GetGameObject();
}
