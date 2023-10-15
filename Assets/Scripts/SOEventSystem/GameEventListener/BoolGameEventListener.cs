using SOGameEventSystem.Events;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class BoolGameEventListener : BaseGameEventListener<bool, BoolGameEvent, UnityEvent<bool>> { }
}
