using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_Float_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/Float Game Event")]
    public class FloatGameEvent : BaseGameEvent<float>
    {
        [ContextMenu("Raise(1.0f)")]
        public void TestRaise()
        {
            Raise(1f);
        }
    }
}
