using SOGameEventSystem.Events;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class FloatGameEventListener : BaseGameEventListener<float, FloatGameEvent, UnityEvent<float>> { }
}
