using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProp : MonoBehaviour
{
    [Header("ID")]
    public int id;
    [System.Serializable]
    public enum PropType
    {
        OneHanded,
        TwoHanded,
        Medium,
    }
    public PropType propType;

    [Header("Stats")]
    public int hitCount;
    public int damage;

    [Header("Status")]
    public bool isHeld = false;

    // Components
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collider[] colls;

    private void Awake()
    {
        foreach(var pd in GameManager.Instance.settings.propDataset)
        {
            if(pd.id == id)
            {
                hitCount = pd.hitCount;
                damage = pd.damage;
            }
        }

        rb = GetComponent<Rigidbody>();
        colls = GetComponents<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit()
    {
        hitCount--;
        if (hitCount <= 0)
            DestroyProp();
    }

    public void DestroyProp()
    {
        Destroy(gameObject);
    }
}
