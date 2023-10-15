using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_Bool_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/Bool Game Event")]
    public class BoolGameEvent : BaseGameEvent<bool> { }
}
