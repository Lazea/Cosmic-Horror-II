using System.Collections.Generic;
using UnityEngine;

namespace SOGameEventSystem
{
    [CreateAssetMenu(
        fileName = "SOEvent_NewGameEvent",
        menuName = "Scriptable Objects/GameEvent System/Game Event")]
    public class BaseGameEvent : ScriptableObject
    {
        private List<IGameEventListener> listeners = new List<IGameEventListener>();

        [ContextMenu("Raise()")]
        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised();
        }

        public void Register(IGameEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(IGameEventListener listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }

    public class BaseGameEvent<T> : ScriptableObject
    {
        private List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

        public void Raise(T item)
        {
            for(int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(item);
        }

        public void Register(IGameEventListener<T> listener)
        {
            if(!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(IGameEventListener<T> listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }

    public class BaseGameEvent<T1, T2> : ScriptableObject
    {
        private List<IGameEventListener<T1, T2>> listeners = new List<IGameEventListener<T1, T2>>();

        public void Raise(T1 item1, T2 item2)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(item1, item2);
        }

        public void Register(IGameEventListener<T1, T2> listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(IGameEventListener<T1, T2> listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }

    public class BaseGameEvent<T1, T2, T3> : ScriptableObject
    {
        private List<IGameEventListener<T1, T2, T3>> listeners = new List<IGameEventListener<T1, T2, T3>>();

        public void Raise(T1 item1, T2 item2, T3 item3)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(item1, item2, item3);
        }

        public void Register(IGameEventListener<T1, T2, T3> listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(IGameEventListener<T1, T2, T3> listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }
}
