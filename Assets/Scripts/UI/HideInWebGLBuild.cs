using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInWebGLBuild : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL
        Destroy(gameObject);
#endif
    }
}
