using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BarrierCleanup : MonoBehaviour
{
    ProBuilderMesh m_proBuilderMesh;
    MeshRenderer m_Renderer;
    MeshFilter m_Filter;

    private void Awake()
    {
        m_proBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Renderer = GetComponent<MeshRenderer>();
        m_Filter = GetComponent<MeshFilter>();

        if(m_proBuilderMesh != null )
            Destroy(m_proBuilderMesh);
        if(m_Renderer != null)
            Destroy(m_Renderer);
        if(m_Filter != null)
            Destroy(m_Filter);
    }
}
