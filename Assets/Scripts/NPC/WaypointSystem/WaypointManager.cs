using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Color color = Color.green;
    public float radius = 0.3f;

    public Waypoint[] waypoints;

    private void Awake()
    {
        SetupWaypoints();
    }

    [ContextMenu("Setup Waypoints List")]
    public void SetupWaypoints()
    {
        ClearWaypoints();

        waypoints = new Waypoint[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypoints[i] = new Waypoint();
            waypoints[i].transform = transform.GetChild(i);
        }

        if(waypoints.Length > 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if(i + 1 >= waypoints.Length)
                    waypoints[i].nextWaypoint = waypoints[0];
                else
                    waypoints[i].nextWaypoint = waypoints[i+1];

                if(i - 1 < 0)
                    waypoints[i].previousWaypoint = waypoints[waypoints.Length - 1];
                else
                    waypoints[i].previousWaypoint = waypoints[i - 1];
            }
        }
    }

    [ContextMenu("Clear Waypoints List")]
    public void ClearWaypoints()
    {
        waypoints = null;
    }

    public Waypoint GetRandomWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Length)];
    }

    public Waypoint GetClosestWaypoint(Vector3 position)
    {
        Waypoint closestWaypoint = null;
        float closestDistance = float.MaxValue;

        foreach (Waypoint waypoint in waypoints)
        {
            float distance = Vector3.Distance(waypoint.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypoint = waypoint;
            }
        }

        return closestWaypoint;
    }

    public Waypoint GetNextWaypoint(Waypoint currentWaypoint)
    {
        return currentWaypoint.nextWaypoint;
    }

    public Waypoint GetPreviousWaypoint(Waypoint currentWaypoint)
    {
        return currentWaypoint.previousWaypoint;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null)
            return;

        if (waypoints.Length <= 0)
            return;

        Gizmos.color = color;
        foreach(Waypoint w in waypoints)
        {
            Gizmos.DrawSphere(w.transform.position, radius);
            if(w.nextWaypoint != null)
            {
                Gizmos.DrawLine(
                    w.transform.position,
                    w.nextWaypoint.transform.position);
            }
        }
    }
}