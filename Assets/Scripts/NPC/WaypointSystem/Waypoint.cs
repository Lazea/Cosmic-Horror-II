using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Transform transform;
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;
}
