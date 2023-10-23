using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCsManager : Singleton<NPCsManager>
{
    public Player Player
    {
        get { return GameManager.Instance.player; }
    }
    [SerializeField]
    WaypointManager[] waypointManagers;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region [NavMesh Positioning]
    public static Vector3 GetNavMeshPositionNearPoint(Vector3 point, float radius)
    {
        Vector2 randPoint = Random.insideUnitCircle;
        Vector3 _point = new Vector3(randPoint.x, point.y, randPoint.y) * radius;

        return GetNavMeshPosition(_point, radius * 2f);
    }

    public static Vector3 GetNavMeshPosition(Vector3 point, float checkRadius)
    {
        int tires = 0;
        while (tires < 3)
        {
            tires++;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(
                point,
                out hit,
                checkRadius,
                NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return point;
    }
    #endregion

    #region [Perception]
    public static bool IsPointInSight(
        Vector3 position,
        Vector3 forward,
        Vector3 point,
        float sightFOV,
        float sightRange,
        LayerMask coverMask)
    {
        Vector3 disp = point - position;
        if(disp.magnitude <= sightRange)
        {
            Vector3 dir = disp.normalized;
            float angle = Vector3.Angle(forward, dir);
            if(Mathf.Abs(angle) <= sightFOV * 0.5f)
            {
                Ray ray = new Ray(position, dir);
                return !Physics.Raycast(
                    ray,
                    sightRange,
                    coverMask);
            }

            return false;
        }
        else
        {
            return false;
        }    
    }

    public static bool IsPointInRange(
        Vector3 position,
        Vector3 point,
        float range)
    {
        return Vector3.Distance(position, point) <= range;
    }
    #endregion

    public WaypointManager[] GetAllWaypointManagers()
    {
        return waypointManagers;
    }

    public WaypointManager GetWaypointManager(int i)
    {
        return waypointManagers[i];
    }
}
