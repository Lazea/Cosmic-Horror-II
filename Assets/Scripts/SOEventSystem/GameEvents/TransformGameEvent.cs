using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_Transform_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/Transform Game Event")]
    public class TransformGameEvent : BaseGameEvent<Transform> { }
}
