using SOGameEventSystem.Events;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class StringGameEventListener : BaseGameEventListener<string, StringGameEvent, UnityEvent<string>> { }
}
