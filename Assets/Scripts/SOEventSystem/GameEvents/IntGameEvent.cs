using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_Int_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/Int Game Event")]
    public class IntGameEvent : BaseGameEvent<int>
    {
        [ContextMenu("Raise(1)")]
        public void TestRaise()
        {
            Raise(1);
        }
    }
}
