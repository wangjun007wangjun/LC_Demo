using System;
using System.Collections.Generic;

namespace BaseFramework
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<string, Delegate> _eventListeners;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();

            _eventListeners = new Dictionary<string, Delegate>();
        }

        public override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();

            RemoveAllListeners();
            _eventListeners = null;
        }

        public void RegistEvent<T>(Action<T> listener) where T : class
        {
            string type = typeof(T).Name;
            if (string.IsNullOrEmpty(type))
            {
                return;
            }

            if (_eventListeners.TryGetValue(type, out Delegate old))
            {
                _eventListeners[type] = Delegate.Combine(old, listener);
            }
            else
            {
                _eventListeners[type] = listener;
            }
        }

        public void UnRegistEvent<T>(Action<T> listener = null) where T : class
        {
            string type = typeof(T).Name;
            if (string.IsNullOrEmpty(type)) return;

            if (!_eventListeners.TryGetValue(type, out Delegate source)) return;

            _eventListeners[type] = Delegate.Remove(source, listener);
            if (listener == null)
            {
                _eventListeners.Remove(type);
            }
        }

        public void RemoveAllListeners()
        {
            _eventListeners.Clear();
        }

        public void DispatchEvent<T>(T evt) where T : class
        {
            string type = typeof(T).Name;
            if (!_eventListeners.TryGetValue(type, out Delegate @delegate)) return;

            if (@delegate is Action<T> action)
            {
                action(evt);
            }
        }
    }
}