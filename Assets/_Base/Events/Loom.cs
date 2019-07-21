using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace BaseFramework
{
    public class Loom : MonoSingleton<Loom>
    {
        private struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        private static int _maxThreads = 8;
        public static int maxThreads
        {
            get => _maxThreads;
            set => _maxThreads = value;
        }

        private static int _numThreads;

        private int _count;

        private readonly List<Action> _actions = new List<Action>();

        private readonly List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        readonly List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        public static void QueueOnMainThread(Action action, float time = 0f)
        {
            if (Math.Abs(time) > 0.001f)
            {
                lock (instance._delayed)
                {
                    instance._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (instance._actions)
                {
                    instance._actions.Add(action);
                }
            }
        }

        public static Thread RunAsync(Action action)
        {
            while (_numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref _numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, action);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
                // ignored
            }
            finally
            {
                Interlocked.Decrement(ref _numThreads);
            }

        }

        readonly List<Action> _currentActions = new List<Action>();

        // Update is called once per frame
        private void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (Action a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (DelayedQueueItem item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (DelayedQueueItem delayed in _currentDelayed)
            {
                delayed.action();
            }
        }
    }
}
