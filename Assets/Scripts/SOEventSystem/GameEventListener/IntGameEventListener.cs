using SOGameEventSystem.Events;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class IntGameEventListener : BaseGameEventListener<int, IntGameEvent, UnityEvent<int>> { }
}
