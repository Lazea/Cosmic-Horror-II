using SOGameEventSystem.Events;
using UnityEngine;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class TransformGameEventListener : BaseGameEventListener<Transform, TransformGameEvent, UnityEvent<Transform>> { }
}
