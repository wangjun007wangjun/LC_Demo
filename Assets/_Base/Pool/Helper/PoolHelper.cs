using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public static class PoolHelper
    {
        private const int DEFAULT_INIT_POOL_SIZE = 10;
        private const int DEFAULT_MAX_POOL_SIZE = 50;

        public static T Create<T>(int initPoolSize = DEFAULT_INIT_POOL_SIZE,
                                  int maxPoolSize = DEFAULT_MAX_POOL_SIZE) where T : class, IRecycleable
        {
            return SimplePool<T>.instance.Init(initPoolSize, maxPoolSize).Create();
        }

        public static T Create<T>(Func<T> createFunc,
                                  int initPoolSize = DEFAULT_INIT_POOL_SIZE,
                                  int maxPoolSize = DEFAULT_MAX_POOL_SIZE) where T : class, IRecycleable
        {
            return SimplePool<T>.instance.Init(createFunc, initPoolSize, maxPoolSize).Create();
        }

        public static T Create<T>(T prefab,
                                  int initPoolSize = DEFAULT_INIT_POOL_SIZE,
                                  int maxPoolSize = DEFAULT_MAX_POOL_SIZE) where T : MonoBehaviour, IRecycleable
        {
            T item = MonoPool<T>.instance
                .Init(() => UnityEngine.Object.Instantiate(prefab),
                      DEFAULT_INIT_POOL_SIZE,
                      DEFAULT_MAX_POOL_SIZE)
                .Create();
            return item;
        }

        public static bool Recycle<T>(this T self) where T : class, IRecycleable
        {
            return self is MonoBehaviour ? MonoPool<T>.instance.Recycle(self) : SimplePool<T>.instance.Recycle(self);
        }
    }
}
