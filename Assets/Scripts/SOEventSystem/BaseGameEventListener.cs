using UnityEngine;
using UnityEngine.Events;

namespace SOGameEventSystem
{
    public class BaseGameEventListener<E, UER> : MonoBehaviour, IGameEventListener where E : BaseGameEvent where UER : UnityEvent
    {
        public E gameEvent;
        public UER response;

        public void OnEventRaised()
        {
            if (response != null)
                response.Invoke();
        }

        void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(this);
        }

        void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(this);
        }
    }

    public class BaseGameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
    {
        public E gameEvent;
        public UER response;

        public virtual void OnEventRaised(T item)
        {
            if (response != null)
                response.Invoke(item);
        }

        void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(this);
        }

        void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(this);
        }
    }

    public class BaseGameEventListener<T1, T2, E, UER> : MonoBehaviour, IGameEventListener<T1, T2> where E : BaseGameEvent<T1, T2> where UER : UnityEvent<T1, T2>
    {
        public E gameEvent;
        public UER response;

        public void OnEventRaised(T1 item1, T2 item2)
        {
            if (response != null)
                response.Invoke(item1, item2);
        }

        void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(this);
        }

        void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(this);
        }
    }

    public class BaseGameEventListener<T1, T2, T3, E, UER> : MonoBehaviour, IGameEventListener<T1, T2, T3> where E : BaseGameEvent<T1, T2, T3> where UER : UnityEvent<T1, T2, T3>
    {
        public E gameEvent;
        public UER response;

        public void OnEventRaised(T1 item1, T2 item2, T3 item3)
        {
            if (response != null)
                response.Invoke(item1, item2, item3);
        }

        void OnEnable()
        {
            if (gameEvent != null)
                gameEvent.Register(this);
        }

        void OnDisable()
        {
            if (gameEvent != null)
                gameEvent.Unregister(this);
        }
    }
}
