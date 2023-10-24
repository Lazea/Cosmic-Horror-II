using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Transform transform;
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;

    public override string ToString()
    {
        return string.Format("{0}: Next Waypoint: {1}; PrevWaypoint: {2}",
            transform.name,
            nextWaypoint.transform.name,
            previousWaypoint.transform.name);
    }
}
