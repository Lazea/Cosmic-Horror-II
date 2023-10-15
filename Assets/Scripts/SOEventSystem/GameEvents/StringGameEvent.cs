using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_String_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/String Game Event")]
    public class StringGameEvent : BaseGameEvent<string>
    {
        [ContextMenu("Raise(\"Test\")")]
        public void TestRaise()
        {
            Raise("Test");
        }
    }
}
